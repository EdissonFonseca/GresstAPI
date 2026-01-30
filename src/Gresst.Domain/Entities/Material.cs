using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Material composition of wastes
/// </summary>
public class Material : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Properties
    public bool IsRecyclable { get; set; }
    public bool IsHazardous { get; set; }
    public string? Category { get; set; } // Metal, Plastic, Glass, Organic, etc.
    
    // Waste Class relationship
    public string? WasteClassId { get; set; }
    public virtual WasteClass? WasteClass { get; set; }
    
    // Navigation properties
    public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();
    public virtual ICollection<PersonMaterial> Persons { get; set; } = new List<PersonMaterial>();
    public virtual ICollection<FacilityMaterial> Facilities { get; set; } = new List<FacilityMaterial>();
    
    // Transformations - Materials that this material can be decomposed/transformed into
    public virtual ICollection<MaterialTransformation> ResultTransformations { get; set; } = new List<MaterialTransformation>();
    
    // Source Transformations - Materials that can be decomposed/transformed into this material
    public virtual ICollection<MaterialTransformation> SourceTransformations { get; set; } = new List<MaterialTransformation>();
    
    // Person Material Treatments - Treatments that persons apply to this material
    public virtual ICollection<PersonMaterialTreatment> PersonTreatments { get; set; } = new List<PersonMaterialTreatment>();
}

