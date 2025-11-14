using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Facility and Material
/// Represents materials that a facility can handle, with pricing and configuration
/// </summary>
public class FacilityMaterial : BaseEntity
{
    /// <summary>
    /// Person who owns/manages this facility-material relationship
    /// </summary>
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Material that the facility can handle
    /// </summary>
    public Guid MaterialId { get; set; }
    public virtual Material Material { get; set; } = null!;
    
    /// <summary>
    /// Facility that can handle this material
    /// </summary>
    public Guid FacilityId { get; set; }
    public virtual Facility Facility { get; set; } = null!;
    
    /// <summary>
    /// Service price for this material at this facility
    /// </summary>
    public decimal? ServicePrice { get; set; }
    
    /// <summary>
    /// Purchase price for this material at this facility
    /// </summary>
    public decimal? PurchasePrice { get; set; }
    
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
    /// Whether the facility handles/manages this material
    /// </summary>
    public bool IsHandled { get; set; } = true;
}

