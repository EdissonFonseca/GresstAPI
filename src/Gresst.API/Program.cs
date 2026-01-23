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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

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
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
// Always enable Swagger for easy API testing
app.UseSwagger();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Gresst API {description.GroupName.ToUpperInvariant()}");
    }

    options.RoutePrefix = string.Empty; // Swagger at root
});

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

// app.UseHttpsRedirection(); // Comentado para desarrollo sin certificado HTTPS
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

