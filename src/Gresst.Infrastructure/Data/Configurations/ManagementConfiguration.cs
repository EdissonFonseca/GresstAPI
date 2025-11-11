using Gresst.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gresst.Infrastructure.Data.Configurations;

public class ManagementConfiguration : IEntityTypeConfiguration<Management>
{
    public void Configure(EntityTypeBuilder<Management> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Quantity)
            .HasPrecision(18, 4);

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(m => m.Waste)
            .WithMany(w => w.Managements)
            .HasForeignKey(m => m.WasteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.ExecutedBy)
            .WithMany()
            .HasForeignKey(m => m.ExecutedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Order)
            .WithMany(o => o.Managements)
            .HasForeignKey(m => m.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Vehicle)
            .WithMany(v => v.Managements)
            .HasForeignKey(m => m.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Treatment)
            .WithMany(t => t.Managements)
            .HasForeignKey(m => m.TreatmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Certificate)
            .WithMany(c => c.Managements)
            .HasForeignKey(m => m.CertificateId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(m => m.AccountId);
        builder.HasIndex(m => m.Code);
        builder.HasIndex(m => m.Type);
        builder.HasIndex(m => m.ExecutedAt);
        builder.HasIndex(m => m.WasteId);
    }
}

