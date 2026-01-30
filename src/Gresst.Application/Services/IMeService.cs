using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for the current user's full context (user, account, person).
/// </summary>
public interface IMeService
{
    Task<MeResponseDto?> GetCurrentContextAsync(CancellationToken cancellationToken = default);
}
