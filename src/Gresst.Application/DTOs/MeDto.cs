namespace Gresst.Application.DTOs;

/// <summary>
/// Summary of the current user's account for "me" context.
/// </summary>
public class AccountSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PersonId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>
/// Summary of the person associated with the current user/account for "me" context.
/// </summary>
public class PersonSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

/// <summary>
/// Current user profile in "me" context (user data without roles; use GET /me/roles for roles).
/// </summary>
public class MeProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    /// <summary>Full name (firstName + lastName).</summary>
    public string Name => $"{FirstName?.Trim()} {LastName?.Trim()}".Trim();
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsActive => Status == "A";
    public string? PersonId { get; set; }
    public DateTime? LastAccess { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Full "me" context: profile, account, person, roles, and permissions in one response.
/// </summary>
public class MeResponseDto
{
    /// <summary>Current user data only (same as GET /me/profile). Does not include roles.</summary>
    public MeProfileDto Profile { get; set; } = null!;

    /// <summary>Account corresponding to the current user.</summary>
    public AccountSummaryDto? Account { get; set; }

    /// <summary>Person corresponding to the current user (user's linked person or account's legal rep).</summary>
    public PersonSummaryDto? Person { get; set; }

    /// <summary>Roles for the current user (same as GET /me/roles).</summary>
    public string[] Roles { get; set; } = Array.Empty<string>();

    /// <summary>Permissions for the current user (same as GET /me/permissions).</summary>
    public IEnumerable<UserPermissionDto> Permissions { get; set; } = Array.Empty<UserPermissionDto>();
}
