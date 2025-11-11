using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.DocumentNumber)
            .HasMaxLength(50);
        
        builder.Property(p => p.Email)
            .HasMaxLength(100);
        
        builder.Property(p => p.Phone)
            .HasMaxLength(50);
        
        builder.Property(p => p.Address)
            .HasMaxLength(500);

        // Relationships
        builder.HasMany(p => p.Licenses)
            .WithOne(l => l.Person)
            .HasForeignKey(l => l.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Facilities)
            .WithOne(f => f.Person)
            .HasForeignKey(f => f.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Vehicles)
            .WithOne(v => v.Person)
            .HasForeignKey(v => v.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.RequestsAsRequester)
            .WithOne(r => r.Requester)
            .HasForeignKey(r => r.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.RequestsAsProvider)
            .WithOne(r => r.Provider)
            .HasForeignKey(r => r.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.AccountId);
        builder.HasIndex(p => p.Email);
        builder.HasIndex(p => p.DocumentNumber);
    }
}

