using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Person, Material, and Treatment
/// Represents which treatment a person applies to a specific material
/// </summary>
public class PersonMaterialTreatment : BaseEntity
{
    /// <summary>
    /// Person who applies this treatment to this material
    /// </summary>
    public string PersonId { get; set; } = string.Empty;
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Material that receives this treatment
    /// </summary>
    public string MaterialId { get; set; } = string.Empty;
    public virtual Material Material { get; set; } = null!;
    
    /// <summary>
    /// Treatment that is applied to this material by this person
    /// </summary>
    public string TreatmentId { get; set; } = string.Empty;
    public virtual Treatment Treatment { get; set; } = null!;
    
    // Note: IsActive is inherited from BaseEntity
}

