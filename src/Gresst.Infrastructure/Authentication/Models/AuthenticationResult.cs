using Gresst.Infrastructure.Authentication;

namespace Gresst.Infrastructure.Authentication.Models;

// ----- Login / refresh token results -----

/// <summary>
/// Result of an authentication attempt (login or token refresh).
/// </summary>
public class AuthenticationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }

    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string AccessTokenType { get; set; } = "Bearer";
    public DateTime? AccessTokenExpiresAt { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    public string SubjectType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    /// <summary>Account display name. Always included in response (empty string if not set).</summary>
    public string AccountName { get; set; } = string.Empty;
    public string AccountPersonId { get; set; } = string.Empty;
    public string PersonId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string[]? Roles { get; set; }
    /// <summary>Hint for clients: access and refresh tokens have been set in cookies (when applicable).</summary>
    public string? CookieMessage { get; set; }
}

// ----- Service token (client credentials) results -----

/// <summary>
/// Internal result of issuing a service token.
/// </summary>
public class ServiceTokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string AccessTokenType { get; set; } = "Bearer";
    public DateTime AccessTokenExpiresAt { get; set; }
    public string SubjectType { get; set; } = ClaimConstants.SubjectTypeService;
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string AccountPersonId { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Response for the service token endpoint (same shape as login for shared fields).
/// </summary>
public class ServiceTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string AccessTokenType { get; set; } = "Bearer";
    public DateTime AccessTokenExpiresAt { get; set; }
    public string SubjectType { get; set; } = ClaimConstants.SubjectTypeService;
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string AccountPersonId { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
    /// <summary>Hint for clients: access token has been set in cookie.</summary>
    public string? CookieMessage { get; set; }
}
