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
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Material that receives this treatment
    /// </summary>
    public Guid MaterialId { get; set; }
    public virtual Material Material { get; set; } = null!;
    
    /// <summary>
    /// Treatment that is applied to this material by this person
    /// </summary>
    public Guid TreatmentId { get; set; }
    public virtual Treatment Treatment { get; set; } = null!;
    
    // Note: IsActive is inherited from BaseEntity
}

