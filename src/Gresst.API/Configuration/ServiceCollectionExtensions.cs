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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Runtime.InteropServices;
using System.Text;

namespace Gresst.API.Configuration;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddGresstLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, _, loggerConfiguration) =>
        {
            var configuredLogPath = context.Configuration["Logging:LogPath"];
            var logsPath = string.IsNullOrWhiteSpace(configuredLogPath)
                ? Path.Combine(AppContext.BaseDirectory, "logs")
                : Path.GetFullPath(configuredLogPath);
            try { Directory.CreateDirectory(logsPath); }
            catch
            {
                logsPath = Path.Combine(AppContext.BaseDirectory, "logs");
                try { Directory.CreateDirectory(logsPath); } catch { /* ignore */ }
            }
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
        return builder;
    }

    public static IServiceCollection AddGresstApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        return services;
    }

    public static IServiceCollection AddGresstMultitenancy(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }

    public static IServiceCollection AddGresstJwtAuthentication(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var config = builder.Configuration;
        var jwtSecret = config["Authentication:JwtSecret"]
            ?? "gresst-jwt-secret-key-change-in-production-must-be-at-least-32-characters-long";
        var key = Encoding.UTF8.GetBytes(jwtSecret);
        var accessTokenCookieName = config["Authentication:AccessTokenCookieName"] ?? "access_token";
        var jwtIssuer = config["Authentication:JwtIssuer"];
        var jwtAudience = config["Authentication:JwtAudience"];

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        context.HttpContext.Items["AuthSource"] = "Bearer";
                        return Task.CompletedTask;
                    }
                    var token = context.Request.Cookies[accessTokenCookieName];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                        context.HttpContext.Items["AuthSource"] = "Cookie";
                    }
                    else
                        context.HttpContext.Items["AuthSource"] = "None";
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Log.Warning(context.Exception, "[JWT] Authentication failed. AuthSource={AuthSource}",
                        context.HttpContext.Items["AuthSource"]?.ToString() ?? "?");
                    return Task.CompletedTask;
                }
            };
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
                ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                ClockSkew = TimeSpan.Zero
            };
        });
        return services;
    }

    public static IServiceCollection AddGresstAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = null;
            options.AddPolicy("HumanOnly", policy =>
                policy.RequireClaim(ClaimConstants.SubjectType, ClaimConstants.SubjectTypeHuman));
            options.AddPolicy("ServiceOnly", policy =>
                policy.RequireClaim(ClaimConstants.SubjectType, ClaimConstants.SubjectTypeService));
            options.AddPolicy(Gresst.Application.Constants.ApiRoles.PolicyAdminOnly, policy =>
                policy.RequireRole(Gresst.Application.Constants.ApiRoles.Admin));
            options.AddPolicy(Gresst.Application.Constants.ApiRoles.PolicyAdminOrUser, policy =>
                policy.RequireRole(Gresst.Application.Constants.ApiRoles.Admin, Gresst.Application.Constants.ApiRoles.User));
        });
        return services;
    }

    public static IServiceCollection AddGresstAuthenticationServices(this IServiceCollection services)
    {
        services.AddHttpClient<ExternalAuthenticationService>();
        services.AddScoped<DatabaseAuthenticationService>();
        services.AddScoped<ExternalAuthenticationService>();
        services.AddScoped<AuthenticationServiceFactory>();
        services.AddScoped<IAuthenticationService>(sp =>
            sp.GetRequiredService<AuthenticationServiceFactory>().GetAuthenticationService());
        return services;
    }

    public static IServiceCollection AddGresstDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<InfrastructureDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sql => sql.UseNetTopologySuite()));
        builder.Services.AddHealthChecks();
        return builder.Services;
    }

    public static IServiceCollection AddGresstMappers(this IServiceCollection services)
    {
        var mappers = new[]
        {
            typeof(AccountMapper), typeof(FacilityMapper), typeof(WasteMapper), typeof(ManagementMapper),
            typeof(PersonMapper), typeof(MaterialMapper), typeof(PersonMaterialMapper), typeof(FacilityMaterialMapper),
            typeof(PersonContactMapper), typeof(PersonMaterialTreatmentMapper), typeof(SupplyMapper), typeof(PersonSupplyMapper),
            typeof(PersonWasteClassMapper), typeof(PackagingMapper), typeof(ServiceMapper), typeof(PersonServiceMapper),
            typeof(WasteClassMapper), typeof(TreatmentMapper), typeof(PersonTreatmentMapper), typeof(RouteMapper), typeof(RouteStopMapper)
        };
        foreach (var mapper in mappers)
            services.AddScoped(mapper);
        return services;
    }

    public static IServiceCollection AddGresstRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IRepository<Facility>, FacilityRepository>();
        services.AddScoped<IRepository<Waste>, WasteRepository>();
        services.AddScoped<IRepository<Management>, ManagementRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IRepository<Person>>(sp => sp.GetRequiredService<IPersonRepository>());
        services.AddScoped<IRepository<Material>, MaterialRepository>();
        services.AddScoped<IRepository<PersonMaterial>, PersonMaterialRepository>();
        services.AddScoped<IRepository<FacilityMaterial>, FacilityMaterialRepository>();
        services.AddScoped<IPersonContactRepository, PersonContactRepository>();
        services.AddScoped<IRepository<PersonContact>>(sp => sp.GetRequiredService<IPersonContactRepository>());
        services.AddScoped<IRepository<Packaging>, PackagingRepository>();
        services.AddScoped<IRepository<Supply>, SupplyRepository>();
        services.AddScoped<IRepository<Service>, ServiceRepository>();
        services.AddScoped<IRepository<PersonService>, PersonServiceRepository>();
        services.AddScoped<IRepository<WasteClass>, WasteClassRepository>();
        services.AddScoped<IRepository<PersonWasteClass>, PersonWasteClassRepository>();
        services.AddScoped<IRepository<Treatment>, TreatmentRepository>();
        services.AddScoped<IRepository<PersonTreatment>, PersonTreatmentRepository>();
        services.AddScoped<IRepository<Gresst.Domain.Entities.Route>, RouteRepository>();
        services.AddScoped<IRepository<Gresst.Domain.Entities.RouteStop>, RouteStopRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericInfraRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    public static IServiceCollection AddGresstApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDataSegmentationService, DataSegmentationService>();
        services.AddScoped<IFacilityService, FacilityService>();
        services.AddScoped<IWasteService, WasteService>();
        services.AddScoped<IManagementService, ManagementService>();
        services.AddScoped<IBalanceService, BalanceService>();
        services.AddScoped<IMaterialService, MaterialService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProviderService, ProviderService>();
        services.AddScoped<IPersonContactService, PersonContactService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IPackagingService, PackagingService>();
        services.AddScoped<ISupplyService, SupplyService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IWasteClassService, WasteClassService>();
        services.AddScoped<ITreatmentService, TreatmentService>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccountRegistrationService, AccountRegistrationService>();
        services.AddScoped<IMeService, MeService>();
        services.AddScoped<Gresst.Application.Services.IAuthorizationService, AuthorizationService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProcessService, ProcessService>();
        return services;
    }

    public static IServiceCollection AddGresstCors(this IHostApplicationBuilder builder)
    {
        var allowedOrigins = builder.Configuration.GetValue<string>("Cors:AllowedOrigins")
            ?? builder.Configuration.GetValue<string>("AllowedHosts") ?? string.Empty;
        var origins = allowedOrigins
            .Split(',', ';', StringSplitOptions.RemoveEmptyEntries)
            .Select(o => o.Trim())
            .Where(o => !string.IsNullOrEmpty(o))
            .Select(o => o.StartsWith("http") ? o : $"https://{o}")
            .ToArray();

        builder.Services.AddCors(options => options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyMethod().AllowAnyHeader();
            if (origins.Length > 0)
            {
                policy.WithOrigins(origins).AllowCredentials();
                // In Development, also allow any localhost origin (e.g. ASP.NET 4.8 app at http://localhost:44300)
                if (builder.Environment.IsDevelopment())
                    policy.SetIsOriginAllowed(origin => origins.Contains(origin) || IsLocalhost(origin));
            }
            else if (builder.Environment.IsDevelopment())
                policy.AllowAnyOrigin();
            else
                policy.SetIsOriginAllowed(_ => false);
        }));
        return builder.Services;
    }

    private static bool IsLocalhost(string? origin)
    {
        if (string.IsNullOrEmpty(origin)) return false;
        return origin.StartsWith("http://localhost", StringComparison.OrdinalIgnoreCase)
            || origin.StartsWith("https://localhost", StringComparison.OrdinalIgnoreCase)
            || origin.StartsWith("http://127.0.0.1", StringComparison.OrdinalIgnoreCase)
            || origin.StartsWith("https://127.0.0.1", StringComparison.OrdinalIgnoreCase);
    }

    public static WebApplicationBuilder ConfigureGresstKestrel(this WebApplicationBuilder builder)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return builder;
        builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(49847, lo =>
            lo.Protocols = HttpProtocols.Http1AndHttp2));
        return builder;
    }
}
