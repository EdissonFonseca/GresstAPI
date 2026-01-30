using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Real-time inventory balance by person/facility/location/waste type
/// </summary>
public class Balance : BaseEntity
{
    // Owner
    public string? PersonId { get; set; }
    public virtual Person? Person { get; set; }
    
    // Location
    public string? FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    
    public string? LocationId { get; set; }
    public virtual Location? Location { get; set; }
    
    // Waste Class
    public string WasteClassId { get; set; } = string.Empty;
    public virtual WasteClass WasteClass { get; set; } = null!;
    
    // Material (optional, for more detail)
    public string? MaterialId { get; set; }
    public virtual Material? Material { get; set; }
    
    // Quantity
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    
    // Status breakdown
    public decimal QuantityGenerated { get; set; }
    public decimal QuantityInTransit { get; set; }
    public decimal QuantityStored { get; set; }
    public decimal QuantityDisposed { get; set; }
    public decimal QuantityTreated { get; set; }
    
    public DateTime LastUpdated { get; set; }
}

