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
    Task<bool> LogoutAsync(Guid userId, string? refreshToken = null, CancellationToken cancellationToken = default);
    Task<Guid> GetAccountIdForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ChangeNameAsync(Guid userId, string name, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default);

}

