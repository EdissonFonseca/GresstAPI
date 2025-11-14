using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for PersonWasteClass entity
/// </summary>
public class PersonWasteClassConfiguration : IEntityTypeConfiguration<PersonWasteClass>
{
    public void Configure(EntityTypeBuilder<PersonWasteClass> builder)
    {
        builder.HasKey(pwc => pwc.Id);
        
        // Relationships
        builder.HasOne(pwc => pwc.Person)
            .WithMany(p => p.WasteClasses)
            .HasForeignKey(pwc => pwc.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(pwc => pwc.WasteClass)
            .WithMany(wc => wc.Persons)
            .HasForeignKey(pwc => pwc.WasteClassId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(pwc => pwc.AccountId);
        builder.HasIndex(pwc => pwc.PersonId);
        builder.HasIndex(pwc => pwc.WasteClassId);
        
        // Composite index for unique constraint (Person + WasteClass per Account)
        builder.HasIndex(pwc => new { pwc.PersonId, pwc.WasteClassId, pwc.AccountId })
            .IsUnique();
    }
}

