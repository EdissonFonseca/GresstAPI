using Gresst.Application.DTOs;
using Gresst.Infrastructure.Authentication.Models;

namespace Gresst.Infrastructure.Authentication;

/// <summary>
/// Common interface for all authentication providers
/// </summary>
public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<(bool,string)> IsUserAuthorizedForInterfaceAsync(string interfaz, string email, string token, CancellationToken cancellationToken = default);
    Task<(bool, string)> IsGuestAuthorizedForInterfaceAsync(string interfaz, string token, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<bool> IsValidRefreshTokenAsync(string email, string token, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(string userId, string? refreshToken = null, CancellationToken cancellationToken = default);
    Task<string> GetAccountIdForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ChangeNameAsync(string userId, string name, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(string userId, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Request a password reset for the given email. Always returns success to avoid email enumeration.
    /// </summary>
    Task<bool> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset password using the token sent by email. Returns true if token was valid and password was updated.
    /// </summary>
    Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);
}

