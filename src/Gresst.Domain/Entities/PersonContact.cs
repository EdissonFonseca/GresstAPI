using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Person and Contact
/// Represents contacts (employees, contractors, etc.) associated with a person
/// Contacts are also Persons but without vehicles, materials, etc.
/// </summary>
public class PersonContact : BaseEntity
{
    /// <summary>
    /// Person who has this contact (the main person: account person, client, provider)
    /// </summary>
    public string PersonId { get; set; } = string.Empty;
    public virtual Person Person { get; set; } = null!;
    
    /// <summary>
    /// Contact person (employee, contractor, etc.)
    /// This is also a Person but without vehicles, materials, etc.
    /// </summary>
    public string ContactId { get; set; } = string.Empty;
    public virtual Person Contact { get; set; } = null!;
    
    /// <summary>
    /// Relationship type code (e.g., "EM" for Employee, "CT" for Contractor)
    /// </summary>
    public string RelationshipType { get; set; } = string.Empty;
    
    /// <summary>
    /// Start date of the relationship
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// End date of the relationship
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Status of the contact relationship
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Whether reconciliation is required for this contact
    /// </summary>
    public bool? RequiresReconciliation { get; set; }
    
    /// <summary>
    /// Whether to send emails to this contact
    /// </summary>
    public bool? SendEmail { get; set; }
    
    /// <summary>
    /// Contact email (can override Person.Email)
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Contact phone (can override Person.Phone)
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Contact secondary phone
    /// </summary>
    public string? Phone2 { get; set; }
    
    /// <summary>
    /// Contact address (can override Person.Address)
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Contact name (can override Person.Name for display purposes)
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Contact job title/position
    /// </summary>
    public string? JobTitle { get; set; }
    
    /// <summary>
    /// Contact webpage
    /// </summary>
    public string? WebPage { get; set; }
    
    /// <summary>
    /// Contact signature
    /// </summary>
    public string? Signature { get; set; }
    
    /// <summary>
    /// Location ID for this contact
    /// </summary>
    public string? LocationId { get; set; }
    public virtual Location? Location { get; set; }
    
    /// <summary>
    /// Additional notes about this contact
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Additional data (JSON format)
    /// </summary>
    public string? AdditionalData { get; set; }
}

