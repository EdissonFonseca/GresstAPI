using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for FacilityTreatment entity
/// Note: This entity is not yet mapped to a database table, but the configuration is ready for when it is added
/// </summary>
public class FacilityTreatmentConfiguration : IEntityTypeConfiguration<FacilityTreatment>
{
    public void Configure(EntityTypeBuilder<FacilityTreatment> builder)
    {
        builder.HasKey(ft => ft.Id);
        
        // Relationships
        builder.HasOne(ft => ft.Person)
            .WithMany(p => p.FacilityTreatments)
            .HasForeignKey(ft => ft.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(ft => ft.Facility)
            .WithMany(f => f.Treatments)
            .HasForeignKey(ft => ft.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(ft => ft.Treatment)
            .WithMany(t => t.Facilities)
            .HasForeignKey(ft => ft.TreatmentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Properties
        builder.Property(ft => ft.CapacityUnit)
            .HasMaxLength(20);
        
        builder.Property(ft => ft.MaxCapacity)
            .HasPrecision(18, 4);
        
        // Indexes
        builder.HasIndex(ft => ft.AccountId);
        builder.HasIndex(ft => ft.PersonId);
        builder.HasIndex(ft => ft.FacilityId);
        builder.HasIndex(ft => ft.TreatmentId);
        
        // Composite index for unique constraint (when table is created)
        builder.HasIndex(ft => new { ft.PersonId, ft.FacilityId, ft.TreatmentId })
            .IsUnique();
    }
}

