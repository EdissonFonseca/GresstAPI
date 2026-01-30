using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Inventory adjustments and transfers
/// </summary>
public class Adjustment : BaseEntity
{
    public string AdjustmentNumber { get; set; } = string.Empty;
    public string AdjustmentType { get; set; } = string.Empty; // Correction, Transfer, Loss, Found
    public DateTime AdjustmentDate { get; set; }
    
    // Waste Type
    public string WasteClassId { get; set; } = string.Empty;
    public virtual WasteClass WasteClass { get; set; } = null!;
    
    // Quantity
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    
    // Origin (for transfers)
    public string? OriginLocationId { get; set; }
    public virtual Location? OriginLocation { get; set; }
    
    public string? OriginFacilityId { get; set; }
    public virtual Facility? OriginFacility { get; set; }
    
    // Destination (for transfers)
    public string? DestinationLocationId { get; set; }
    public virtual Location? DestinationLocation { get; set; }
    
    public string? DestinationFacilityId { get; set; }
    public virtual Facility? DestinationFacility { get; set; }
    
    // Performed by
    public string PerformedById { get; set; } = string.Empty;
    public virtual Person PerformedBy { get; set; } = null!;
    
    // Reason
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

