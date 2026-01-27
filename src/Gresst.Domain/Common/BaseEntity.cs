namespace Gresst.Domain.Common;

public abstract class BaseEntity
{
    public string Id { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty; // Multitenant support
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;
}

