using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Certificates of waste management operations
/// </summary>
public class Certificate : BaseEntity
{
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    
    // Issuer
    public Guid IssuedById { get; set; }
    public virtual Person IssuedBy { get; set; } = null!;
    
    // Recipient
    public Guid IssuedToId { get; set; }
    public virtual Person IssuedTo { get; set; } = null!;
    
    // Certificate Type
    public string CertificateType { get; set; } = string.Empty; // Collection, Transport, Disposal, Treatment, etc.
    
    // Related entities
    public Guid? OrderId { get; set; }
    public virtual Order? Order { get; set; }
    
    // Content
    public string Description { get; set; } = string.Empty;
    public string? DocumentUrl { get; set; }
    public string? VerificationCode { get; set; }
    
    // Status
    public bool IsValid { get; set; } = true;
    public DateTime? RevokedDate { get; set; }
    public string? RevokedReason { get; set; }
    
    // Navigation properties
    public virtual ICollection<Management> Managements { get; set; } = new List<Management>();
}

