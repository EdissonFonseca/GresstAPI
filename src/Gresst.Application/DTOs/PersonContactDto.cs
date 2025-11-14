namespace Gresst.Application.DTOs;

/// <summary>
/// DTO for PersonContact relationship
/// </summary>
public class PersonContactDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    
    // Person (main person: account person, client, provider)
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    
    // Contact (employee, contractor, etc.)
    public Guid ContactId { get; set; }
    public string? ContactName { get; set; }
    public string? ContactDocumentNumber { get; set; }
    
    // Relationship
    public string RelationshipType { get; set; } = string.Empty;
    public string? RelationshipTypeName { get; set; }
    
    // Dates
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Status
    public string? Status { get; set; }
    public bool? RequiresReconciliation { get; set; }
    public bool? SendEmail { get; set; }
    
    // Contact Information (can override Person properties)
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public string? Address { get; set; }
    public string? Name { get; set; }
    public string? JobTitle { get; set; }
    public string? WebPage { get; set; }
    public string? Signature { get; set; }
    
    // Location
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    
    // Additional
    public string? Notes { get; set; }
    public string? AdditionalData { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a PersonContact
/// </summary>
public class CreatePersonContactDto
{
    public Guid PersonId { get; set; }
    public Guid ContactId { get; set; }
    public string RelationshipType { get; set; } = string.Empty;
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public bool? RequiresReconciliation { get; set; }
    public bool? SendEmail { get; set; }
    
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public string? Address { get; set; }
    public string? Name { get; set; }
    public string? JobTitle { get; set; }
    public string? WebPage { get; set; }
    public string? Signature { get; set; }
    public Guid? LocationId { get; set; }
    public string? Notes { get; set; }
    public string? AdditionalData { get; set; }
}

/// <summary>
/// DTO for updating a PersonContact
/// </summary>
public class UpdatePersonContactDto
{
    public Guid PersonId { get; set; }
    public Guid ContactId { get; set; }
    public string? RelationshipType { get; set; }
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public bool? RequiresReconciliation { get; set; }
    public bool? SendEmail { get; set; }
    
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public string? Address { get; set; }
    public string? Name { get; set; }
    public string? JobTitle { get; set; }
    public string? WebPage { get; set; }
    public string? Signature { get; set; }
    public Guid? LocationId { get; set; }
    public string? Notes { get; set; }
    public string? AdditionalData { get; set; }
    public bool? IsActive { get; set; }
}

