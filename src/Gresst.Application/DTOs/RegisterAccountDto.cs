namespace Gresst.Application.DTOs;

/// <summary>
/// Request to register a new account with its first administrator user.
/// </summary>
public class RegisterAccountRequest
{
    /// <summary>Account/organization name.</summary>
    public string AccountName { get; set; } = string.Empty;

    /// <summary>Administrator display name.</summary>
    public string AdminName { get; set; } = string.Empty;

    /// <summary>Optional administrator last name.</summary>
    public string? AdminLastName { get; set; }

    /// <summary>Administrator email (used for login). Must be unique.</summary>
    public string AdminEmail { get; set; } = string.Empty;

    /// <summary>Administrator password.</summary>
    public string AdminPassword { get; set; } = string.Empty;

    /// <summary>Password confirmation (must match AdminPassword).</summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>Optional existing person id to link as legal rep; if null, a minimal person is created for the admin.</summary>
    public string? PersonId { get; set; }
}

/// <summary>
/// Result of account registration: created account id and admin user.
/// </summary>
public class RegisterAccountResultDto
{
    public string AccountId { get; set; } = null!;
    public UserDto AdminUser { get; set; } = null!;
}
