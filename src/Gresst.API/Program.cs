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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Gresst Waste Management API", 
        Version = "v1",
        Description = "Complete waste management system with traceability, inventory, and certificates"
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

// Repositories - Register specific repositories with mappers
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IRepository<Facility>, FacilityRepository>();
builder.Services.AddScoped<IRepository<Waste>, WasteRepository>();
builder.Services.AddScoped<IRepository<Management>, ManagementRepository>();
builder.Services.AddScoped<IRepository<Person>, PersonRepository>();

// Generic repository for entities without specific mappers yet
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericInfraRepository<>));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Services
builder.Services.AddScoped<IDataSegmentationService, DataSegmentationService>();
builder.Services.AddScoped<IFacilityService, FacilityService>();
builder.Services.AddScoped<IWasteService, WasteService>();
builder.Services.AddScoped<IManagementService, ManagementService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

// Planning Services (To be implemented)
// builder.Services.AddScoped<IRequestService, RequestService>();
// builder.Services.AddScoped<IOrderService, OrderService>();
// builder.Services.AddScoped<IRouteService, RouteService>();

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
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gresst API v1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

