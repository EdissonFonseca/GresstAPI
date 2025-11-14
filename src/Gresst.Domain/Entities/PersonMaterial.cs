using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Person and Material
/// Represents materials that a person can generate, receive, treat, or deliver
/// </summary>
public class PersonMaterial : BaseEntity
{
    /// <summary>
    /// Person who has this material relationship
    /// </summary>
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Material associated with the person
    /// </summary>
    public Guid MaterialId { get; set; }
    public virtual Material Material { get; set; } = null!;
    
    /// <summary>
    /// Custom name for this material in the context of this person
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Service price for this material
    /// </summary>
    public decimal? ServicePrice { get; set; }
    
    /// <summary>
    /// Purchase price for this material
    /// </summary>
    public decimal? PurchasePrice { get; set; }
    
    /// <summary>
    /// Sale price for this material
    /// </summary>
    public decimal? SalePrice { get; set; }
    
    /// <summary>
    /// Weight of the material
    /// </summary>
    public decimal? Weight { get; set; }
    
    /// <summary>
    /// Volume of the material
    /// </summary>
    public decimal? Volume { get; set; }
    
    /// <summary>
    /// Emission compensation factor
    /// </summary>
    public decimal? EmissionCompensationFactor { get; set; }
    
    /// <summary>
    /// Reference code for this material
    /// </summary>
    public string? Reference { get; set; }
    
    /// <summary>
    /// Packaging ID (if applicable)
    /// </summary>
    public Guid? PackagingId { get; set; }
    public virtual Packaging? Packaging { get; set; }
}

