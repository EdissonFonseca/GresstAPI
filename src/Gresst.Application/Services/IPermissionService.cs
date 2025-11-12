using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing user permissions and authorization
/// Renamed to IPermissionService to avoid conflict with Microsoft.AspNetCore.Authorization.IAuthorizationService
/// </summary>
public interface IPermissionService
{
    // Options management
    Task<IEnumerable<OptionDto>> GetAllOptionsAsync(CancellationToken cancellationToken = default);
    Task<OptionDto?> GetOptionByIdAsync(string optionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OptionDto>> GetOptionsByParentAsync(string? parentId, CancellationToken cancellationToken = default);
    
    // User permissions
    Task<IEnumerable<UserPermissionDto>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserPermissionDto?> GetUserPermissionAsync(Guid userId, string optionId, CancellationToken cancellationToken = default);
    Task<bool> AssignPermissionAsync(AssignPermissionDto dto, CancellationToken cancellationToken = default);
    Task<bool> RevokePermissionAsync(Guid userId, string optionId, CancellationToken cancellationToken = default);
    Task<bool> UpdatePermissionAsync(Guid userId, string optionId, AssignPermissionDto dto, CancellationToken cancellationToken = default);
    
    // Permission checking
    Task<bool> UserHasPermissionAsync(Guid userId, string optionId, PermissionFlags permission, CancellationToken cancellationToken = default);
    Task<bool> CurrentUserHasPermissionAsync(string optionId, PermissionFlags permission, CancellationToken cancellationToken = default);
}

