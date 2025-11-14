using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for MaterialTransformation entity
/// </summary>
public class MaterialTransformationConfiguration : IEntityTypeConfiguration<MaterialTransformation>
{
    public void Configure(EntityTypeBuilder<MaterialTransformation> builder)
    {
        builder.HasKey(mt => mt.Id);
        
        // Relationships
        builder.HasOne(mt => mt.SourceMaterial)
            .WithMany(m => m.SourceTransformations)
            .HasForeignKey(mt => mt.SourceMaterialId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(mt => mt.ResultMaterial)
            .WithMany(m => m.ResultTransformations)
            .HasForeignKey(mt => mt.ResultMaterialId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Properties
        builder.Property(mt => mt.RelationshipType)
            .IsRequired()
            .HasMaxLength(2);
        
        builder.Property(mt => mt.Percentage)
            .HasPrecision(5, 2);
        
        builder.Property(mt => mt.Quantity)
            .HasPrecision(18, 2);
        
        builder.Property(mt => mt.Weight)
            .HasPrecision(18, 2);
        
        builder.Property(mt => mt.Volume)
            .HasPrecision(18, 2);
        
        // Indexes
        builder.HasIndex(mt => mt.SourceMaterialId);
        builder.HasIndex(mt => mt.ResultMaterialId);
        builder.HasIndex(mt => mt.RelationshipType);
        
        // Composite index for unique constraint (matches DB composite key)
        builder.HasIndex(mt => new { mt.SourceMaterialId, mt.RelationshipType, mt.ResultMaterialId })
            .IsUnique();
    }
}

