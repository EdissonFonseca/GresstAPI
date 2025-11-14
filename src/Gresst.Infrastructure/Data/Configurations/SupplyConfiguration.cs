using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Supply entity
/// </summary>
public class SupplyConfiguration : IEntityTypeConfiguration<Supply>
{
    public void Configure(EntityTypeBuilder<Supply> builder)
    {
        builder.ToTable("Supply"); // Domain table name
        
        builder.HasKey(s => s.Id);
        
        // Properties
        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(s => s.Description)
            .HasMaxLength(500);
        
        builder.Property(s => s.CategoryUnitId)
            .IsRequired()
            .HasMaxLength(20);
        
        // Self-referencing relationship for hierarchical structure
        builder.HasOne(s => s.ParentSupply)
            .WithMany(s => s.ChildSupplies)
            .HasForeignKey(s => s.ParentSupplyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(s => s.Code);
        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.ParentSupplyId);
        builder.HasIndex(s => s.IsPublic);
    }
}

