using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing user permissions and authorization
/// Renamed to IPermissionService to avoid conflict with Microsoft.AspNetCore.Authorization.IAuthorizationService
/// </summary>
public interface IAuthorizationService
{
    // Options management
    Task<IEnumerable<OptionDto>> GetAllOptionsAsync(CancellationToken cancellationToken = default);
    Task<OptionDto?> GetOptionByIdAsync(string optionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OptionDto>> GetOptionsByParentAsync(string? parentId, CancellationToken cancellationToken = default);
    
    // User permissions
    Task<IEnumerable<UserPermissionDto>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserPermissionDto?> GetUserPermissionAsync(string userId, string optionId, CancellationToken cancellationToken = default);
    Task<bool> AssignPermissionAsync(AssignPermissionDto dto, CancellationToken cancellationToken = default);
    Task<bool> RevokePermissionAsync(string userId, string optionId, CancellationToken cancellationToken = default);
    Task<bool> UpdatePermissionAsync(string userId, string optionId, AssignPermissionDto dto, CancellationToken cancellationToken = default);
    
    // Permission checking
    Task<bool> UserHasPermissionAsync(string userId, string optionId, PermissionFlags permission, CancellationToken cancellationToken = default);
    Task<bool> CurrentUserHasPermissionAsync(string optionId, PermissionFlags permission, CancellationToken cancellationToken = default);
}

