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
    public Guid WasteTypeId { get; set; }
    public virtual WasteType WasteType { get; set; } = null!;
    
    // Quantity
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    
    // Origin (for transfers)
    public Guid? OriginLocationId { get; set; }
    public virtual Location? OriginLocation { get; set; }
    
    public Guid? OriginFacilityId { get; set; }
    public virtual Facility? OriginFacility { get; set; }
    
    // Destination (for transfers)
    public Guid? DestinationLocationId { get; set; }
    public virtual Location? DestinationLocation { get; set; }
    
    public Guid? DestinationFacilityId { get; set; }
    public virtual Facility? DestinationFacility { get; set; }
    
    // Performed by
    public Guid PerformedById { get; set; }
    public virtual Person PerformedBy { get; set; } = null!;
    
    // Reason
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

