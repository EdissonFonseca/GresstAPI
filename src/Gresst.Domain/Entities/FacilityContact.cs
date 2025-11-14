using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Facility and Contact
/// Represents contacts (employees, contractors, etc.) associated with a facility
/// Contacts are Persons but without vehicles, materials, etc.
/// </summary>
public class FacilityContact : BaseEntity
{
    /// <summary>
    /// Facility that has this contact
    /// </summary>
    public Guid FacilityId { get; set; }
    public virtual Facility Facility { get; set; } = null!;
    
    /// <summary>
    /// Contact person (employee, contractor, etc.)
    /// This is a Person but without vehicles, materials, etc.
    /// </summary>
    public Guid ContactId { get; set; }
    public virtual Person Contact { get; set; } = null!;
    
    /// <summary>
    /// Relationship type code (e.g., "EM" for Employee, "CT" for Contractor)
    /// </summary>
    public string RelationshipType { get; set; } = string.Empty;
    
    /// <summary>
    /// Additional notes about this contact
    /// </summary>
    public string? Notes { get; set; }
    
    // Note: IsActive is inherited from BaseEntity
}

