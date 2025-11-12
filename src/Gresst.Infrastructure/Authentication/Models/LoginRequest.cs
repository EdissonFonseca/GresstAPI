namespace Gresst.Infrastructure.Authentication.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid? PersonId { get; set; }
    public Guid AccountId { get; set; }
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

