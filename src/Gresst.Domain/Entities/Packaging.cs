using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Packaging types for waste containers
/// </summary>
public class Packaging : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Type
    public string PackagingType { get; set; } = string.Empty; // Drum, Bag, Container, Tank, etc.
    
    // Capacity
    public decimal? Capacity { get; set; }
    public string? CapacityUnit { get; set; }
    
    // Properties
    public bool IsReusable { get; set; }
    public string? Material { get; set; }
    
    // UN Packaging codes (for hazardous waste)
    public string? UNPackagingCode { get; set; }
    
    // Navigation properties
    public virtual ICollection<Waste> Wastes { get; set; } = new List<Waste>();
    public virtual ICollection<PersonMaterial> PersonMaterials { get; set; } = new List<PersonMaterial>();
    public virtual ICollection<PersonPackaging> Persons { get; set; } = new List<PersonPackaging>();
}

