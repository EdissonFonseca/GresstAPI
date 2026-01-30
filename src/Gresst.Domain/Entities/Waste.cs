using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Individual waste item with complete traceability
/// </summary>
public class Waste : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Waste Class and Classification
    public string WasteClassId { get; set; } = string.Empty;
    public virtual WasteClass WasteClass { get; set; } = null!;
    
    // Quantity
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    
    // Status and Location
    public WasteStatus Status { get; set; }
    public string? CurrentLocationId { get; set; }
    public virtual Location? CurrentLocation { get; set; }
    
    public string? CurrentFacilityId { get; set; }
    public virtual Facility? CurrentFacility { get; set; }
    
    // Generator
    public string GeneratorId { get; set; } = string.Empty;
    public virtual Person Generator { get; set; } = null!;
    public DateTime GeneratedAt { get; set; }
    
    // Current Owner/Responsible
    public string? CurrentOwnerId { get; set; }
    public virtual Person? CurrentOwner { get; set; }
    
    // Packaging
    public string? PackagingId { get; set; }
    public virtual Packaging? Packaging { get; set; }
    
    // Properties
    public bool IsHazardous { get; set; }
    public bool IsAvailableInBank { get; set; } // For Waste Bank
    public string? BankDescription { get; set; }
    public decimal? BankPrice { get; set; }
    
    // Tracking
    public string? BatchNumber { get; set; }
    public string? ContainerNumber { get; set; }
    
    // Navigation properties
    public virtual ICollection<Management> Managements { get; set; } = new List<Management>();
    public virtual ICollection<WasteTransformation> TransformationsAsSource { get; set; } = new List<WasteTransformation>();
    public virtual ICollection<WasteTransformation> TransformationsAsResult { get; set; } = new List<WasteTransformation>();
}

