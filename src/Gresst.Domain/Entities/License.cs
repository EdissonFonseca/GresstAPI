using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Licenses for persons/facilities to operate
/// </summary>
public class License : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Collection, Transport, Disposal, Treatment, Storage
    public string IssuingAuthorityId { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public new bool IsActive { get; set; } = true;
    public string? DocumentUrl { get; set; }
    List<string>? ProcedureIds { get; set; } = new List<string>();
}

