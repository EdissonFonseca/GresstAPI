using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Identity;

/// <summary>
/// User account for authentication and authorization
/// </summary>
public class Account : BaseEntity
{
    public required string PartyId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty; // FK to User.Id, assuming one-to-one relationship 
    public AccountRole Role { get; set; } = AccountRole.Generator;
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    List<User> Users { get; set; } = new List<User>();
}
