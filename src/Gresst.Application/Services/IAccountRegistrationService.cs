using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for registering a new account with its first administrator user.
/// </summary>
public interface IAccountRegistrationService
{
    /// <summary>
    /// Creates a new account, optionally a legal-rep person, and the first admin user.
    /// Returns the account id and the created user, or null if validation fails (e.g. email already in use).
    /// </summary>
    Task<RegisterAccountResultDto?> RegisterAccountAsync(RegisterAccountRequest request, CancellationToken cancellationToken = default);
}
