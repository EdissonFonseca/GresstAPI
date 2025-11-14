using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for FacilityContact entity
/// </summary>
public class FacilityContactConfiguration : IEntityTypeConfiguration<FacilityContact>
{
    public void Configure(EntityTypeBuilder<FacilityContact> builder)
    {
        builder.HasKey(fc => fc.Id);
        
        // Relationships
        builder.HasOne(fc => fc.Facility)
            .WithMany(f => f.Contacts)
            .HasForeignKey(fc => fc.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(fc => fc.Contact)
            .WithMany(p => p.FacilityContacts)
            .HasForeignKey(fc => fc.ContactId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Properties
        builder.Property(fc => fc.RelationshipType)
            .IsRequired()
            .HasMaxLength(2);
        
        builder.Property(fc => fc.Notes)
            .HasMaxLength(500);
        
        // Indexes
        builder.HasIndex(fc => fc.FacilityId);
        builder.HasIndex(fc => fc.ContactId);
        builder.HasIndex(fc => fc.RelationshipType);
        
        // Composite index for unique constraint (matches DB composite key)
        builder.HasIndex(fc => new { fc.FacilityId, fc.RelationshipType, fc.ContactId })
            .IsUnique();
    }
}

