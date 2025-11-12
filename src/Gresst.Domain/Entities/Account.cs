using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Account (Cuenta) - Organization that participates in waste management
/// Represents a company or entity that generates, collects, transports, or treats waste
/// </summary>
public class Account : BaseEntity
{
    /// <summary>
    /// Account/Organization name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Unique business code
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// Role of this account in waste management business
    /// </summary>
    public AccountRole Role { get; set; }
    
    /// <summary>
    /// Business status of the account
    /// </summary>
    public AccountStatus Status { get; set; }
    
    // Business Relations
    
    /// <summary>
    /// Legal representative or owner person
    /// </summary>
    public Guid PersonId { get; set; }
    public virtual Person? Person { get; set; }
    
    /// <summary>
    /// Parent account (for organizational hierarchies)
    /// </summary>
    public Guid? ParentAccountId { get; set; }
    public virtual Account? ParentAccount { get; set; }
    
    // Business Capabilities (computed from Role)
    
    /// <summary>
    /// Can generate waste
    /// </summary>
    public bool IsGenerator => Role == AccountRole.Generator || Role == AccountRole.Both;
    
    /// <summary>
    /// Can operate as logistics provider (collect, transport, treat)
    /// </summary>
    public bool IsOperator => Role == AccountRole.Operator || Role == AccountRole.Both;
    
    /// <summary>
    /// Is the account active for business operations
    /// </summary>
    public bool IsActiveForBusiness => Status == AccountStatus.Active;
    
    // Navigation properties
    public virtual ICollection<Facility>? Facilities { get; set; }
    public virtual ICollection<Waste>? Wastes { get; set; }
}
