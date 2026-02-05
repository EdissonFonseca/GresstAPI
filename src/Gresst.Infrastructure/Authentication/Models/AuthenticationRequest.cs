namespace Gresst.Infrastructure.Authentication.Models;

// ----- Login -----

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

public class ExternalLoginRequest
{
    public string Provider { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

// ----- Register -----

/// <summary>
/// Request to register a new user under an existing account.
/// </summary>
public class RegisterRequest
{
    public string AccountId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? PersonId { get; set; }
}

// ----- Token refresh / validation -----

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

// ----- Service token (client credentials) -----

/// <summary>
/// Request for obtaining a token as a service/client (machine-to-machine). Client Credentials flow only.
/// </summary>
public class ServiceTokenRequest
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

// ----- Password reset -----

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
