using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Facility and WasteClass
/// Represents waste classes that a facility can handle/manage
/// </summary>
public class FacilityWasteClass : BaseEntity
{
    /// <summary>
    /// Facility that can handle this waste class
    /// </summary>
    public Guid FacilityId { get; set; }
    public virtual Facility Facility { get; set; } = null!;
    
    /// <summary>
    /// Waste class that the facility can handle
    /// </summary>
    public Guid WasteClassId { get; set; }
    public virtual WasteClass WasteClass { get; set; } = null!;
    
    /// <summary>
    /// Relationship type code (e.g., "AC" for Accepted, "RE" for Rejected, "TR" for Treated)
    /// </summary>
    public string RelationshipType { get; set; } = string.Empty;
    
    // Note: IsActive is inherited from BaseEntity
}

