using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for PersonMaterialTreatment entity
/// </summary>
public class PersonMaterialTreatmentConfiguration : IEntityTypeConfiguration<PersonMaterialTreatment>
{
    public void Configure(EntityTypeBuilder<PersonMaterialTreatment> builder)
    {
        builder.HasKey(pmt => pmt.Id);
        
        // Relationships
        builder.HasOne(pmt => pmt.Person)
            .WithMany(p => p.MaterialTreatments)
            .HasForeignKey(pmt => pmt.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(pmt => pmt.Material)
            .WithMany(m => m.PersonTreatments)
            .HasForeignKey(pmt => pmt.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(pmt => pmt.Treatment)
            .WithMany(t => t.PersonMaterialTreatments)
            .HasForeignKey(pmt => pmt.TreatmentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(pmt => pmt.AccountId);
        builder.HasIndex(pmt => pmt.PersonId);
        builder.HasIndex(pmt => pmt.MaterialId);
        builder.HasIndex(pmt => pmt.TreatmentId);
        
        // Composite index for unique constraint (Person + Material + Treatment per Account)
        builder.HasIndex(pmt => new { pmt.PersonId, pmt.MaterialId, pmt.TreatmentId, pmt.AccountId })
            .IsUnique();
    }
}

