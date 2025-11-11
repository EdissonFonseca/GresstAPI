namespace Gresst.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; } // Multitenant support
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;
}

