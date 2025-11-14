using Gresst.Domain.Common;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Data;

public class GreesstDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public GreesstDbContext(DbContextOptions<GreesstDbContext> options, ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    // DbSets
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Waste> Wastes => Set<Waste>();
    public DbSet<WasteClass> WasteClasses => Set<WasteClass>();
    public DbSet<Classification> Classifications => Set<Classification>();
    public DbSet<Management> Managements => Set<Management>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Request> Requests => Set<Request>();
    public DbSet<RequestItem> RequestItems => Set<RequestItem>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<License> Licenses => Set<License>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Balance> Balances => Set<Balance>();
    public DbSet<Adjustment> Adjustments => Set<Adjustment>();
    public DbSet<WasteTransformation> WasteTransformations => Set<WasteTransformation>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Packaging> Packagings => Set<Packaging>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<RouteStop> RouteStops => Set<RouteStop>();
    // User removed - not a domain entity, only for authentication in Infrastructure
    public DbSet<Rate> Rates => Set<Rate>();
    public DbSet<FacilityTreatment> FacilityTreatments => Set<FacilityTreatment>();
    public DbSet<FacilityContact> FacilityContacts => Set<FacilityContact>();
    public DbSet<FacilityWasteClass> FacilityWasteClasses => Set<FacilityWasteClass>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GreesstDbContext).Assembly);

        // Global query filters for multitenant support
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var accountIdProperty = Expression.Property(parameter, nameof(BaseEntity.AccountId));
                
                // Create lambda: e => e.AccountId == currentAccountId
                if (_currentUserService != null)
                {
                    var currentAccountId = Expression.Constant(_currentUserService.GetCurrentAccountId());
                    var equals = Expression.Equal(accountIdProperty, currentAccountId);
                    var lambda = Expression.Lambda(equals, parameter);
                    
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Set audit fields
        var entries = ChangeTracker.Entries<BaseEntity>();
        var currentAccountId = _currentUserService?.GetCurrentAccountId() ?? Guid.Empty;
        var currentUsername = _currentUserService?.GetCurrentUsername() ?? "System";

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Id = Guid.NewGuid();
                    entry.Entity.AccountId = currentAccountId;
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = currentUsername;
                    entry.Entity.IsActive = true;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUsername;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

