using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Certificates of waste management operations
/// </summary>
public class Certificate : BaseEntity
{
    public string? Number { get; set; } = string.Empty;
    public string IssuerId { get; set; } = string.Empty;
    public CertificateStatus Status { get; set; } = CertificateStatus.Pending;
    public OperationType? Type { get; set; }
    public string? ProcedureId { get; set; }
    public string? OrderId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? DocumentUrl { get; set; }
    public string? VerificationCode { get; set; }
    
    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiryAt { get; set; }    
    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }
    // Navigation properties

    public ICollection<string>? WasteIds { get; set; }
    public ICollection<string>? PartyIds { get; set; }
    public ICollection<CertificateEvent> Events { get; set; } = new List<CertificateEvent>();

    public void Apply(CertificateEvent certEvent)
    {
        Status = certEvent.ToStatus;
        if (certEvent.ToStatus == CertificateStatus.Issued)
        {
            IssuedAt = certEvent.OccurredAt;
            Number = certEvent.CertificateNumber;
            DocumentUrl = certEvent.DocumentUrl;
        }
        Events.Add(certEvent);
    }}

