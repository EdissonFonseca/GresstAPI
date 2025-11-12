using Gresst.Infrastructure.Authentication.Models;

namespace Gresst.Infrastructure.Authentication;

/// <summary>
/// Common interface for all authentication providers
/// </summary>
public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(Guid userId, string? refreshToken = null, CancellationToken cancellationToken = default);
    Task<Guid> GetAccountIdForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

