using Gresst.API.Services;
using Gresst.Application.Services;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Gresst.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

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
});

// HttpContext for multitenancy
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

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
builder.Services.AddScoped<FacilityMapper>();
builder.Services.AddScoped<WasteMapper>();
builder.Services.AddScoped<ManagementMapper>();
builder.Services.AddScoped<PersonMapper>();

// Repositories - Register specific repositories with mappers
builder.Services.AddScoped<IRepository<Facility>, FacilityRepository>();
builder.Services.AddScoped<IRepository<Waste>, WasteRepository>();
builder.Services.AddScoped<IRepository<Management>, ManagementRepository>();
builder.Services.AddScoped<IRepository<Person>, PersonRepository>();

// Generic repository for entities without specific mappers yet
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericInfraRepository<>));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Services
builder.Services.AddScoped<IFacilityService, FacilityService>();
builder.Services.AddScoped<IWasteService, WasteService>();
builder.Services.AddScoped<IManagementService, ManagementService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();

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
app.UseAuthorization();
app.MapControllers();

app.Run();

