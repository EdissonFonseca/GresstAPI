using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for PersonSupply entity
/// </summary>
public class PersonSupplyConfiguration : IEntityTypeConfiguration<PersonSupply>
{
    public void Configure(EntityTypeBuilder<PersonSupply> builder)
    {
        builder.HasKey(ps => ps.Id);
        
        // Relationships
        builder.HasOne(ps => ps.Person)
            .WithMany(p => p.Supplies)
            .HasForeignKey(ps => ps.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(ps => ps.Supply)
            .WithMany(s => s.Persons)
            .HasForeignKey(ps => ps.SupplyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Properties
        builder.Property(ps => ps.Price)
            .HasPrecision(18, 2);
        
        // Indexes
        builder.HasIndex(ps => ps.AccountId);
        builder.HasIndex(ps => ps.PersonId);
        builder.HasIndex(ps => ps.SupplyId);
        
        // Composite index for unique constraint (Person + Supply per Account)
        builder.HasIndex(ps => new { ps.PersonId, ps.SupplyId, ps.AccountId })
            .IsUnique();
    }
}

