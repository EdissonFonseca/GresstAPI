using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Facility and Treatment
/// Represents treatments that a facility can perform
/// </summary>
public class FacilityTreatment : BaseEntity
{
    /// <summary>
    /// Person who owns/manages this facility-treatment relationship
    /// </summary>
    public string PersonId { get; set; } = string.Empty;
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Facility that can perform this treatment
    /// </summary>
    public string FacilityId { get; set; } = string.Empty;
    public virtual Facility Facility { get; set; } = null!;
    
    /// <summary>
    /// Treatment that the facility can perform
    /// </summary>
    public string TreatmentId { get; set; } = string.Empty;
    public virtual Treatment Treatment { get; set; } = null!;
    
    /// <summary>
    /// Whether the facility can perform this treatment
    /// </summary>
    public bool CanPerform { get; set; } = true;
    
    /// <summary>
    /// Maximum capacity for this treatment at this facility
    /// </summary>
    public decimal? MaxCapacity { get; set; }
    
    /// <summary>
    /// Capacity unit (kg, m3, etc.)
    /// </summary>
    public string? CapacityUnit { get; set; }
    
    // Note: IsActive is inherited from BaseEntity
}

