using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Account (Cuenta) - Multitenant account entity
/// </summary>
public class Account : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "S" for logistic operator, "N" for waste generator
    
    // Relations
    public Guid PersonId { get; set; }
    public virtual Person? Person { get; set; }
    
    public Guid AdministratorId { get; set; }
    public virtual User? Administrator { get; set; }
    
    // Configuration
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();
    
    // Status
    public string? Status { get; set; }
    public bool PermissionsBySite { get; set; }
}
