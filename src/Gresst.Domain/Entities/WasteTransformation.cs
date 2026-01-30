using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Records waste transformations (conversion, decomposition, grouping)
/// </summary>
public class WasteTransformation : BaseEntity
{
    public string TransformationNumber { get; set; } = string.Empty;
    public TransformationType Type { get; set; }
    public DateTime TransformationDate { get; set; }
    
    // Source waste
    public string SourceWasteId { get; set; } = string.Empty;
    public virtual Waste SourceWaste { get; set; } = null!;
    
    public decimal SourceQuantity { get; set; }
    public UnitOfMeasure SourceUnit { get; set; }
    
    // Result waste
    public string ResultWasteId { get; set; } = string.Empty;
    public virtual Waste ResultWaste { get; set; } = null!;
    
    public decimal ResultQuantity { get; set; }
    public UnitOfMeasure ResultUnit { get; set; }
    
    // Process
    public string? TreatmentId { get; set; }
    public virtual Treatment? Treatment { get; set; }
    
    // Performed at
    public string? FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    
    // Performed by
    public string PerformedById { get; set; } = string.Empty;
    public virtual Person PerformedBy { get; set; } = null!;
    
    // Details
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

