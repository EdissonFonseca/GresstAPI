using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

public class WasteConfiguration : IEntityTypeConfiguration<Waste>
{
    public void Configure(EntityTypeBuilder<Waste> builder)
    {
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Code)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(w => w.Description)
            .HasMaxLength(500);

        builder.Property(w => w.Quantity)
            .HasPrecision(18, 4);

        builder.Property(w => w.BankPrice)
            .HasPrecision(18, 2);

        builder.Property(w => w.BatchNumber)
            .HasMaxLength(50);

        builder.Property(w => w.ContainerNumber)
            .HasMaxLength(50);

        // Relationships
        builder.HasOne(w => w.WasteClass)
            .WithMany(wt => wt.Wastes)
            .HasForeignKey(w => w.WasteClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Generator)
            .WithMany()
            .HasForeignKey(w => w.GeneratorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.CurrentOwner)
            .WithMany()
            .HasForeignKey(w => w.CurrentOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.CurrentLocation)
            .WithMany()
            .HasForeignKey(w => w.CurrentLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.CurrentFacility)
            .WithMany()
            .HasForeignKey(w => w.CurrentFacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Packaging)
            .WithMany(p => p.Wastes)
            .HasForeignKey(w => w.PackagingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(w => w.Managements)
            .WithOne(m => m.Waste)
            .HasForeignKey(m => m.WasteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(w => w.AccountId);
        builder.HasIndex(w => w.Code);
        builder.HasIndex(w => w.Status);
        builder.HasIndex(w => w.IsAvailableInBank);
        builder.HasIndex(w => w.GeneratorId);
        builder.HasIndex(w => w.WasteClassId);
    }
}

