using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Relationship between Person and Contact
/// Represents contacts (employees, contractors, etc.) associated with a person
/// Contacts are also Persons but without vehicles, materials, etc.
/// </summary>
public class Contact : Party
{
    public PartyRelationType? ContactRole { get; set; }
    
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
    /// Additional notes about this contact
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Additional data (JSON format)
    /// </summary>
    public string? AdditionalData { get; set; }
}

