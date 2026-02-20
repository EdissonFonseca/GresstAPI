namespace Gresst.Application.DTOs;

/// <summary>
/// DTO for PersonContact relationship
/// </summary>
public class PartyDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    
    public string? DocumentNumber { get; set; }
    public ICollection<PartyRelationType> Roles { get; set; } = new List<PartyRelationType>();

    // Status
    public string? Status { get; set; }
    public bool? RequiresReconciliation { get; set; }
    public bool? SendEmail { get; set; }
    
    // Contact Information (can override Person properties)
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public string? Address { get; set; }
    public string? JobTitle { get; set; }
    public string? WebPage { get; set; }
    public string? Signature { get; set; }
    
    // Location
    public string? LocationId { get; set; }
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
public class CreatePartyDto
{
    public string PersonId { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
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
    public string? LocationId { get; set; }
    public string? Notes { get; set; }
    public string? AdditionalData { get; set; }
}

/// <summary>
/// DTO for updating a PersonContact
/// </summary>
public class UpdatePartyDto
{
    public string PersonId { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
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
    public string? LocationId { get; set; }
    public string? Notes { get; set; }
    public string? AdditionalData { get; set; }
    public bool? IsActive { get; set; }
}

