using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.LicensePlate)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(v => v.VehicleType)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(v => v.Model)
            .HasMaxLength(100);
        
        builder.Property(v => v.Make)
            .HasMaxLength(100);
        
        builder.Property(v => v.CapacityUnit)
            .HasMaxLength(20);
        
        builder.Property(v => v.MaxCapacity)
            .HasPrecision(18, 4);
        
        builder.Property(v => v.SpecialEquipment)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(v => v.Person)
            .WithMany(p => p.Vehicles)
            .HasForeignKey(v => v.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(v => v.VirtualFacility)
            .WithMany()
            .HasForeignKey(v => v.VirtualFacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(v => v.AccountId);
        builder.HasIndex(v => v.LicensePlate);
        builder.HasIndex(v => v.PersonId);
        builder.HasIndex(v => v.VirtualFacilityId);
    }
}

