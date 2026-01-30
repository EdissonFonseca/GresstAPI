using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for user management (not authentication)
/// </summary>
public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetUsersByAccountAsync(string accountId, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateUserProfileAsync(string userId, UpdateUserProfileDto dto, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> ActivateUserAsync(string userId, CancellationToken cancellationToken = default);
}

