using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Gresst.API.Configuration;
using Gresst.API.Endpoints;
using Gresst.API.Services;
using Gresst.Application.Services;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Authentication;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Gresst.Infrastructure.Repositories;
using Gresst.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Runtime.InteropServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    var logsPath = Path.Combine(AppContext.BaseDirectory, "logs");
    loggerConfiguration
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            Path.Combine(logsPath, "log-.txt"),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 31,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
});

// Minimal API - no controllers
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// HttpContext for multitenancy
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Authentication - JWT Configuration
var jwtSecret = builder.Configuration["Authentication:JwtSecret"] 
    ?? "gresst-jwt-secret-key-change-in-production-must-be-at-least-32-characters-long";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Simplificado: no requerir HTTPS metadata
    options.SaveToken = true;

    // If no Bearer token in Authorization header, read token from cookie (e.g. for browser-based clients)
    var accessTokenCookieName = builder.Configuration["Authentication:AccessTokenCookieName"] ?? "access_token";
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            var hasBearer = !string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);
            if (hasBearer)
            {
                context.HttpContext.Items["AuthSource"] = "Bearer";
                Serilog.Log.Information("[JWT] Token from Authorization header (Bearer)");
                return Task.CompletedTask;
            }

            var token = context.Request.Cookies[accessTokenCookieName];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
                context.HttpContext.Items["AuthSource"] = "Cookie";
                Serilog.Log.Information("[JWT] Token from cookie {CookieName} (length={Len})", accessTokenCookieName, token.Length);
            }
            else
            {
                context.HttpContext.Items["AuthSource"] = "None";
                var cookieHeader = context.Request.Headers.Cookie.FirstOrDefault();
                Serilog.Log.Warning("[JWT] No Bearer and no cookie {CookieName}. Request Cookie header present: {HasCookieHeader}", accessTokenCookieName, !string.IsNullOrEmpty(cookieHeader));
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Serilog.Log.Warning(context.Exception, "[JWT] Authentication failed. AuthSource={AuthSource}", context.HttpContext.Items["AuthSource"]?.ToString() ?? "?");
            return Task.CompletedTask;
        }
    };

    var jwtIssuer = builder.Configuration["Authentication:JwtIssuer"];
    var jwtAudience = builder.Configuration["Authentication:JwtAudience"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // Solo validar Issuer/Audience si están configurados
        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    // Configurar política por defecto: permitir acceso anónimo (los controladores usarán [Authorize] explícitamente)
    options.FallbackPolicy = null; // No requerir autenticación por defecto

    // Differentiate human (interactive user) vs service (machine/client) for policy-based authorization
    options.AddPolicy("HumanOnly", policy =>
        policy.RequireClaim(Gresst.Infrastructure.Authentication.ClaimConstants.SubjectType, Gresst.Infrastructure.Authentication.ClaimConstants.SubjectTypeHuman));
    options.AddPolicy("ServiceOnly", policy =>
        policy.RequireClaim(Gresst.Infrastructure.Authentication.ClaimConstants.SubjectType, Gresst.Infrastructure.Authentication.ClaimConstants.SubjectTypeService));

    // Account-scoped roles (see docs/ROLES.md and Gresst.Application.Constants.ApiRoles)
    options.AddPolicy(Gresst.Application.Constants.ApiRoles.PolicyAdminOnly, policy =>
        policy.RequireRole(Gresst.Application.Constants.ApiRoles.Admin));
    options.AddPolicy(Gresst.Application.Constants.ApiRoles.PolicyAdminOrUser, policy =>
        policy.RequireRole(Gresst.Application.Constants.ApiRoles.Admin, Gresst.Application.Constants.ApiRoles.User));
});

// Authentication Services - Dual provider (Database + External)
builder.Services.AddHttpClient<ExternalAuthenticationService>();
builder.Services.AddScoped<DatabaseAuthenticationService>();
builder.Services.AddScoped<ExternalAuthenticationService>();
builder.Services.AddScoped<AuthenticationServiceFactory>();
builder.Services.AddScoped<IAuthenticationService>(sp =>
{
    var factory = sp.GetRequiredService<AuthenticationServiceFactory>();
    return factory.GetAuthenticationService();
});

// Database - Using scaffolded context
builder.Services.AddDbContext<InfrastructureDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.UseNetTopologySuite(); // Support for geography/geometry types
        });
});

// Health checks (required for MapHealthChecks; add .AddDbContextCheck or .AddSqlServer for readiness)
builder.Services.AddHealthChecks();

// Mappers - Register all mappers
builder.Services.AddScoped<AccountMapper>();
builder.Services.AddScoped<FacilityMapper>();
builder.Services.AddScoped<WasteMapper>();
builder.Services.AddScoped<ManagementMapper>();
builder.Services.AddScoped<PersonMapper>();
builder.Services.AddScoped<MaterialMapper>();
builder.Services.AddScoped<PersonMaterialMapper>();
builder.Services.AddScoped<FacilityMaterialMapper>();
builder.Services.AddScoped<PersonContactMapper>();
builder.Services.AddScoped<PersonMaterialTreatmentMapper>();
builder.Services.AddScoped<SupplyMapper>();
builder.Services.AddScoped<PersonSupplyMapper>();
builder.Services.AddScoped<PersonWasteClassMapper>();
builder.Services.AddScoped<PackagingMapper>();
builder.Services.AddScoped<ServiceMapper>();
builder.Services.AddScoped<PersonServiceMapper>();
builder.Services.AddScoped<WasteClassMapper>();
builder.Services.AddScoped<TreatmentMapper>();
builder.Services.AddScoped<PersonTreatmentMapper>();
builder.Services.AddScoped<RouteMapper>();
builder.Services.AddScoped<RouteStopMapper>();

// Repositories - Register specific repositories with mappers
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IRepository<Facility>, FacilityRepository>();
builder.Services.AddScoped<IRepository<Waste>, WasteRepository>();
builder.Services.AddScoped<IRepository<Management>, ManagementRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IRepository<Person>>(sp => sp.GetRequiredService<IPersonRepository>());
builder.Services.AddScoped<IRepository<Material>, MaterialRepository>();
builder.Services.AddScoped<IRepository<PersonMaterial>, PersonMaterialRepository>();
builder.Services.AddScoped<IRepository<FacilityMaterial>, FacilityMaterialRepository>();
builder.Services.AddScoped<IPersonContactRepository, PersonContactRepository>();
builder.Services.AddScoped<IRepository<PersonContact>>(sp => sp.GetRequiredService<IPersonContactRepository>());
builder.Services.AddScoped<IRepository<Packaging>, PackagingRepository>();
builder.Services.AddScoped<IRepository<Supply>, SupplyRepository>();
builder.Services.AddScoped<IRepository<Service>, ServiceRepository>();
builder.Services.AddScoped<IRepository<PersonService>, PersonServiceRepository>();
builder.Services.AddScoped<IRepository<WasteClass>, WasteClassRepository>();
builder.Services.AddScoped<IRepository<PersonWasteClass>, PersonWasteClassRepository>();
builder.Services.AddScoped<IRepository<Treatment>, TreatmentRepository>();
builder.Services.AddScoped<IRepository<PersonTreatment>, PersonTreatmentRepository>();
builder.Services.AddScoped<IRepository<Gresst.Domain.Entities.Route>, RouteRepository>();
builder.Services.AddScoped<IRepository<Gresst.Domain.Entities.RouteStop>, RouteStopRepository>();
builder.Services.AddScoped<Gresst.Application.Services.IRequestRepository, RequestRepository>();

// Generic repository for entities without specific mappers yet
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericInfraRepository<>));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Services
builder.Services.AddScoped<IDataSegmentationService, DataSegmentationService>();
builder.Services.AddScoped<IFacilityService, FacilityService>();
builder.Services.AddScoped<IWasteService, WasteService>();
builder.Services.AddScoped<IManagementService, ManagementService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProviderService, ProviderService>();
builder.Services.AddScoped<IPersonContactService, PersonContactService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IPackagingService, PackagingService>();
builder.Services.AddScoped<ISupplyService, SupplyService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IWasteClassService, WasteClassService>();
builder.Services.AddScoped<ITreatmentService, TreatmentService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountRegistrationService, AccountRegistrationService>();
builder.Services.AddScoped<IMeService, MeService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

// Planning Services
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProcessService, ProcessService>();

// CORS
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // En desarrollo: permitir todo
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    }
    else
    {
        // En producción: solo dominios específicos
        var allowedHosts = builder.Configuration.GetValue<string>("AllowedHosts") ?? string.Empty;
        var origins = allowedHosts.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(origin => origin.Trim())
            .Where(origin => !string.IsNullOrEmpty(origin))
            .Select(origin => origin.StartsWith("http") ? origin : $"https://{origin}")
            .ToArray();

        options.AddPolicy("AllowAll", policy =>
        {
            if (origins.Length > 0)
            {
                policy.WithOrigins(origins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
            else
            {
                // Fallback: si no hay dominios configurados, no permitir nada
                policy.AllowAnyMethod()
                      .AllowAnyHeader();
            }
        });
    }
});

// Detectar si estamos en Windows
var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

// Configurar Kestrel: solo HTTPS en Windows, solo HTTP en Mac
if (!isWindows)
{
    // En Mac, forzar solo HTTP eliminando HTTPS de las URLs
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Limpiar todas las configuraciones y solo escuchar en HTTP
        options.ListenLocalhost(49847, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSerilogRequestLogging();

// Exception handler
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error != null)
            {
                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("GlobalException");
                logger.LogError(exceptionHandlerPathFeature.Error, 
                    "Unhandled exception: {Path}", context.Request.Path);
            }
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        });
    });
}

// HTTPS redirection: siempre en producción, condicional en desarrollo
if (app.Environment.IsProduction() || isWindows)
{
    app.UseHttpsRedirection();
}

// Legacy: rewrite any /api/... (except /api/v1/...) to /api/v1/... so clients can use paths without the version segment
// Note: rewrite middleware strips the leading slash from Path before regex matching, so pattern must not start with /
var rewriteOptions = new RewriteOptions()
    .AddRewrite(@"^api/(?!v1/)(.*)$", "/api/v1/$1", skipRemainingRules: true);
app.UseRewriter(rewriteOptions);

app.UseCors("AllowAll");

// Swagger: configure before Authentication/Authorization for unauthenticated access
var enableSwagger = app.Configuration.GetValue<bool>("Swagger:Enabled", true);
if (enableSwagger)
{
    app.UseSwagger();
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerUI(options =>
    {
        var descriptions = apiVersionDescriptionProvider.ApiVersionDescriptions;
        if (descriptions.Count == 0)
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gresst API v1");
        else
        {
            foreach (var description in descriptions)
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    $"Gresst API {description.GroupName.ToUpperInvariant()}");
        }
        options.RoutePrefix = string.Empty; // Swagger at root
    });
}

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint: /health (anonymous; for load balancers, k8s, monitoring)
app.MapHealthChecks("/health");

// Minimal API endpoints (replaces MapControllers)
app.MapApiEndpoints();

app.Run();