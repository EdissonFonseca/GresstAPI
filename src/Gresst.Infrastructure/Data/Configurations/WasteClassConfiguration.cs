using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for WasteClass entity
/// </summary>
public class WasteClassConfiguration : IEntityTypeConfiguration<WasteClass>
{
    public void Configure(EntityTypeBuilder<WasteClass> builder)
    {
        builder.ToTable("WasteClass"); // Domain table name
        
        builder.HasKey(wc => wc.Id);
        
        // Properties
        builder.Property(wc => wc.Code)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(wc => wc.Name)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(wc => wc.Description)
            .HasMaxLength(500);
        
        builder.Property(wc => wc.PhysicalState)
            .HasMaxLength(50);
        
        // One-to-one optional relationship with Treatment
        // Note: The database has a many-to-many table (TipoResiduo_Tratamiento),
        // but we're modeling it as one-to-one in the domain.
        // The mapper will handle the conversion, taking only the first/only treatment per WasteClass.
        builder.HasOne(wc => wc.Treatment)
            .WithOne(t => t.WasteClass)
            .HasForeignKey<WasteClass>(wc => wc.TreatmentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Note: This relationship is not directly mapped to a database foreign key.
        // Instead, it uses the TipoResiduo_Tratamiento junction table,
        // and the mapper will ensure only one treatment is associated per WasteClass.
        
        // Indexes
        builder.HasIndex(wc => wc.Code);
        builder.HasIndex(wc => wc.Name);
        builder.HasIndex(wc => wc.TreatmentId);
    }
}

