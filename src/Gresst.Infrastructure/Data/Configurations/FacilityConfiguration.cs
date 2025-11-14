using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Code)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(f => f.Description)
            .HasMaxLength(500);

        builder.Property(f => f.FacilityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Address)
            .HasMaxLength(500);

        builder.Property(f => f.Latitude)
            .HasPrecision(10, 8);

        builder.Property(f => f.Longitude)
            .HasPrecision(11, 8);

        builder.Property(f => f.MaxCapacity)
            .HasPrecision(18, 4);

        builder.Property(f => f.CurrentCapacity)
            .HasPrecision(18, 4);

        builder.Property(f => f.CapacityUnit)
            .HasMaxLength(20);

        // Relationships
        builder.HasOne(f => f.Person)
            .WithMany(p => p.Facilities)
            .HasForeignKey(f => f.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.Locations)
            .WithOne(l => l.Facility)
            .HasForeignKey(l => l.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Self-referencing relationship for parent facility
        builder.HasOne(f => f.ParentFacility)
            .WithMany(f => f.ChildFacilities)
            .HasForeignKey(f => f.ParentFacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(f => f.AccountId);
        builder.HasIndex(f => f.Code);
        builder.HasIndex(f => f.PersonId);
        builder.HasIndex(f => f.ParentFacilityId);
        builder.HasIndex(f => f.IsVirtual);
    }
}

