using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for FacilityWasteClass entity
/// </summary>
public class FacilityWasteClassConfiguration : IEntityTypeConfiguration<FacilityWasteClass>
{
    public void Configure(EntityTypeBuilder<FacilityWasteClass> builder)
    {
        builder.HasKey(fwc => fwc.Id);
        
        // Relationships
        builder.HasOne(fwc => fwc.Facility)
            .WithMany(f => f.WasteClasses)
            .HasForeignKey(fwc => fwc.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(fwc => fwc.WasteClass)
            .WithMany(wc => wc.Facilities)
            .HasForeignKey(fwc => fwc.WasteClassId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Properties
        builder.Property(fwc => fwc.RelationshipType)
            .IsRequired()
            .HasMaxLength(2);
        
        // Indexes
        builder.HasIndex(fwc => fwc.FacilityId);
        builder.HasIndex(fwc => fwc.WasteClassId);
        builder.HasIndex(fwc => fwc.RelationshipType);
        
        // Composite index for unique constraint (matches DB composite key)
        builder.HasIndex(fwc => new { fwc.FacilityId, fwc.RelationshipType, fwc.WasteClassId })
            .IsUnique();
    }
}

