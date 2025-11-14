using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Pricing rates for services
/// </summary>
public class Rate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Service Type
    public string ServiceType { get; set; } = string.Empty; // Collection, Transport, Disposal, etc.
    
    // Provider
    public Guid ProviderId { get; set; }
    public virtual Person Provider { get; set; } = null!;
    
    // Waste Class
    public Guid? WasteClassId { get; set; }
    public virtual WasteClass? WasteClass { get; set; }
    
    // Pricing
    public decimal BasePrice { get; set; }
    public string PriceUnit { get; set; } = string.Empty; // per kg, per ton, per trip, flat rate
    
    // Validity
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    
    // Additional costs
    public decimal? MinimumCharge { get; set; }
    public string? AdditionalFees { get; set; } // JSON
}

