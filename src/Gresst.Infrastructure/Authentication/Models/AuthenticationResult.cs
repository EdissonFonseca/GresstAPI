namespace Gresst.Infrastructure.Authentication.Models;

/// <summary>
/// Result of authentication attempt
/// </summary>
public class AuthenticationResult
{
    public bool Success { get; set; }
    
    /// <summary>
    /// JWT Access Token (short-lived, e.g., 15 minutes)
    /// </summary>
    public string? AccessToken { get; set; }
    
    /// <summary>
    /// Refresh Token (long-lived, e.g., 7 days)
    /// Used to get a new AccessToken when it expires
    /// </summary>
    public string? RefreshToken { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string AccountPersonId { get; set; } = string.Empty;
    public string PersonId { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string[]? Roles { get; set; }
    public string? Error { get; set; }
    
    /// <summary>
    /// When the AccessToken expires
    /// </summary>
    public DateTime? AccessTokenExpiresAt { get; set; }
    
    /// <summary>
    /// When the RefreshToken expires
    /// </summary>
    public DateTime? RefreshTokenExpiresAt { get; set; }
    
    // Backward compatibility
    [Obsolete("Use AccessToken instead")]
    public string? Token => AccessToken;
    
    [Obsolete("Use AccessTokenExpiresAt instead")]
    public DateTime? ExpiresAt => AccessTokenExpiresAt;
}

