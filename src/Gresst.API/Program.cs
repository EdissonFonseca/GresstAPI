using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Gresst.API.Configuration;
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
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

// Add services to the container
builder.Services.AddControllers();

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
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = builder.Environment.IsProduction(),
        ValidateAudience = builder.Environment.IsProduction(),
        ValidIssuer = builder.Configuration["Authentication:JwtIssuer"],
        ValidAudience = builder.Configuration["Authentication:JwtAudience"],
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    // Configurar política por defecto: permitir acceso anónimo (los controladores usarán [Authorize] explícitamente)
    options.FallbackPolicy = null; // No requerir autenticación por defecto
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
builder.Services.AddScoped<IClientService, ClientService>();
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

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature is not null)
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("GlobalException");

            var exception = exceptionHandlerPathFeature.Error;
            logger.LogError(exception,
                "Unhandled exception while processing {Path}. Exception: {ExceptionMessage}. StackTrace: {StackTrace}",
                context.Request.Path,
                exception.Message,
                exception.StackTrace);
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    });
});

// HTTPS redirection: siempre en producción, condicional en desarrollo
if (app.Environment.IsProduction() || isWindows)
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

// Configuración de Swagger (debe ir ANTES de Authentication/Authorization cuando no requiere auth)
var enableSwagger = app.Configuration.GetValue<bool>("Swagger:Enabled", true);
// SEGURIDAD: En producción SIEMPRE requerir autenticación (no puede ser sobrescrito por configuración)
// En desarrollo: respetar la configuración Swagger:RequireAuth
var requireAuthForSwagger = app.Environment.IsProduction() 
    ? true  // Producción: hardcoded a true, ignora cualquier configuración por seguridad
    : app.Configuration.GetValue<bool>("Swagger:RequireAuth", false);  // Desarrollo: respetar configuración

// Configurar Swagger primero (sin autenticación si está configurado así)
if (enableSwagger && !requireAuthForSwagger)
{
    // Swagger sin autenticación: configurar ANTES de Authentication/Authorization
    app.UseSwagger();
    
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", 
                $"Gresst API {description.GroupName.ToUpperInvariant()}");
        }
        
        options.RoutePrefix = string.Empty; // Swagger at root
    });
}

// Authentication y Authorization para los controladores
// IMPORTANTE: UseAuthentication y UseAuthorization NO bloquean rutas por defecto
// Solo se aplican cuando hay [Authorize] en los controladores
app.UseAuthentication();
app.UseAuthorization();

if (enableSwagger && requireAuthForSwagger)
{
    // Swagger con autenticación: configurar DESPUÉS de Authentication/Authorization
    app.Use(async (context, next) =>
    {
        // Verificar si la ruta es Swagger o la raíz (donde está SwaggerUI)
        if (context.Request.Path.StartsWithSegments("/swagger") || 
            context.Request.Path == "/" || 
            string.IsNullOrEmpty(context.Request.Path.Value) || 
            context.Request.Path.Value == "/")
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { 
                    error = "Unauthorized", 
                    message = "Swagger requires authentication. Please provide a valid JWT token in the Authorization header." 
                });
                return;
            }
        }
        await next();
    });
    
    app.UseSwagger();
    
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", 
                $"Gresst API {description.GroupName.ToUpperInvariant()}");
        }
        
        options.RoutePrefix = string.Empty; // Swagger at root
        options.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
}

app.MapControllers();

app.Run();
