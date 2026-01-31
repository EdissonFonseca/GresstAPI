using Gresst.Infrastructure.Authentication;

namespace Gresst.Infrastructure.Authentication.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Interface { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Request to register a new user under an existing account.
/// </summary>
public class RegisterRequest
{
    /// <summary>Existing account id the user will belong to.</summary>
    public string AccountId { get; set; } = string.Empty;

    /// <summary>Display name (or use Email as identifier).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional last name.</summary>
    public string? LastName { get; set; }

    /// <summary>Email (used for login). Must be unique.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Password confirmation (must match Password).</summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>Optional person id to link to this user.</summary>
    public string? PersonId { get; set; }
}

public class ExternalLoginRequest
{
    public string Provider { get; set; } = string.Empty; // "Google", "Microsoft", "Auth0", etc.
    public string Token { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Request to validate that a refresh token is still valid (no new tokens issued).
/// </summary>
public class ValidateRefreshTokenRequest
{
    public string Username { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Request for obtaining a token as a service/client (machine-to-machine).
/// Validates interface name + client token; optionally binds to a user by email.
/// </summary>
public class ServiceTokenRequest
{
    /// <summary>Interface/client identifier (e.g. "web-portal", "mobile-app").</summary>
    public string Interface { get; set; } = string.Empty;

    /// <summary>Client token configured for this interface in CuentaInterfaz.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Optional. When provided, token is issued for this user (must match account-interface).</summary>
    public string? Username { get; set; }
}

/// <summary>
/// Response for service token endpoint (access token only; no refresh token for services).
/// </summary>
public class ServiceTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string SubjectType { get; set; } = ClaimConstants.SubjectTypeService;
}

/// <summary>
/// Request for forgot-password: user provides email to receive a reset link/token.
/// </summary>
public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Request for reset-password: user provides the token from email and new password.
/// </summary>
public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

