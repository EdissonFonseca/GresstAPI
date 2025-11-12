using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for user management (not authentication)
/// </summary>
public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetUsersByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto dto, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ActivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

