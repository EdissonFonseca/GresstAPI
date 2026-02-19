using Gresst.Domain.Common;

namespace Gresst.Domain.Identity;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}