namespace Gresst.Application.DTOs;

/// <summary>
/// Summary of the current user's account for "me" context.
/// </summary>
public class AccountSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
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
/// Full "me" context: profile (user data), account, and person in a clear structure.
/// </summary>
public class MeResponseDto
{
    /// <summary>Current user data only (same as GET /me/profile).</summary>
    public UserDto Profile { get; set; } = null!;

    /// <summary>Account corresponding to the current user.</summary>
    public AccountSummaryDto? Account { get; set; }

    /// <summary>Person corresponding to the current user (user's linked person or account's legal rep).</summary>
    public PersonSummaryDto? Person { get; set; }
}
