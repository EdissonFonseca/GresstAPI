using Gresst.Domain.RouteProcesses;
using Gresst.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data;

/// <summary>
/// DbContext for the RouteProcess aggregate (Transport process), its stops, and WasteOperations.
/// Uses the same connection as the main app; can be merged into InfrastructureDbContext later if desired.
/// </summary>
public class RouteProcessDbContext : DbContext
{
    public RouteProcessDbContext(DbContextOptions<RouteProcessDbContext> options)
        : base(options)
    {
    }

    public DbSet<RouteProcess> RouteProcesses => Set<RouteProcess>();
    public DbSet<WasteOperationEntity> WasteOperations => Set<WasteOperationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RouteProcess>(b =>
        {
            b.ToTable("RouteProcesses");
            b.HasKey(r => r.Id);
            b.Property(r => r.VehicleId);
            b.Property(r => r.DriverId);
            b.Property(r => r.Status).HasConversion<string>();
            b.Property(r => r.CreatedAt);
            b.Property(r => r.StartedAt);
            b.Property(r => r.CompletedAt);
            b.Property(r => r.CancelledAt);
            b.Property(r => r.CancellationReason);
            b.HasMany(r => r.Stops)
                .WithOne()
                .HasForeignKey(s => s.RouteProcessId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(r => r.Stops).HasField("_stops").AutoInclude();
        });

        modelBuilder.Entity<RouteStop>(b =>
        {
            b.ToTable("RouteStops");
            b.HasKey(s => s.Id);
            b.Property(s => s.RouteProcessId);
            b.Property(s => s.LocationId);
            b.Property(s => s.Order);
            b.Property(s => s.OperationType).HasConversion<string>();
            b.Property(s => s.ResponsiblePartyId);
            b.Property(s => s.IsCompleted);
            b.Property(s => s.CompletedAt);
            b.Property(s => s.Notes);
        });

        modelBuilder.Entity<WasteOperationEntity>(b =>
        {
            b.ToTable("WasteOperations");
            b.HasKey(e => e.Id);
            b.Property(e => e.Type);
            b.Property(e => e.ProcessId);
            b.Property(e => e.OccurredAt);
            b.Property(e => e.DataJson);
        });
    }
}
