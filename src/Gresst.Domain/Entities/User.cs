using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// System users
/// </summary>
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // Associated Person
    public Guid? PersonId { get; set; }
    public virtual Person? Person { get; set; }
    
    // Roles
    public string Roles { get; set; } = string.Empty; // JSON array: Admin, Manager, Operator, etc.
    
    // Status
    public bool IsLocked { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

