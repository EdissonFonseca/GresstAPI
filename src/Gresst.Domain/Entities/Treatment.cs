using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Treatment types/processes
/// </summary>
public class Treatment : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Treatment Category
    public string Category { get; set; } = string.Empty; // Physical, Chemical, Biological, Thermal
    
    // Process details
    public string? ProcessDescription { get; set; }
    public decimal? EstimatedDuration { get; set; } // in hours
    
    // Applicable waste types
    public string? ApplicableWasteTypes { get; set; } // JSON array of waste type IDs
    
    // Results
    public bool ProducesNewWaste { get; set; }
    public string? ResultingWasteTypes { get; set; } // JSON array
    
    // Navigation properties
    public virtual ICollection<Management> Managements { get; set; } = new List<Management>();
    public virtual ICollection<WasteTransformation> Transformations { get; set; } = new List<WasteTransformation>();
}

