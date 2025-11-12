using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>
    /// Get all system options/modules
    /// </summary>
    [HttpGet("options")]
    [ProducesResponseType(typeof(IEnumerable<OptionDto>), 200)]
    public async Task<ActionResult<IEnumerable<OptionDto>>> GetAllOptions(CancellationToken cancellationToken)
    {
        var options = await _permissionService.GetAllOptionsAsync(cancellationToken);
        return Ok(options);
    }

    /// <summary>
    /// Get option by ID
    /// </summary>
    [HttpGet("options/{optionId}")]
    [ProducesResponseType(typeof(OptionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OptionDto>> GetOption(string optionId, CancellationToken cancellationToken)
    {
        var option = await _permissionService.GetOptionByIdAsync(optionId, cancellationToken);
        
        if (option == null)
            return NotFound(new { error = "Option not found" });

        return Ok(option);
    }

    /// <summary>
    /// Get child options of a parent option
    /// </summary>
    [HttpGet("options/{parentId}/children")]
    [ProducesResponseType(typeof(IEnumerable<OptionDto>), 200)]
    public async Task<ActionResult<IEnumerable<OptionDto>>> GetChildOptions(string parentId, CancellationToken cancellationToken)
    {
        var options = await _permissionService.GetOptionsByParentAsync(parentId, cancellationToken);
        return Ok(options);
    }

    /// <summary>
    /// Get all permissions for a user
    /// </summary>
    [HttpGet("users/{userId}/permissions")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<UserPermissionDto>), 200)]
    public async Task<ActionResult<IEnumerable<UserPermissionDto>>> GetUserPermissions(Guid userId, CancellationToken cancellationToken)
    {
        var permissions = await _permissionService.GetUserPermissionsAsync(userId, cancellationToken);
        return Ok(permissions);
    }

    /// <summary>
    /// Get current user's permissions
    /// </summary>
    [HttpGet("me/permissions")]
    [ProducesResponseType(typeof(IEnumerable<UserPermissionDto>), 200)]
    public async Task<ActionResult<IEnumerable<UserPermissionDto>>> GetMyPermissions(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var permissions = await _permissionService.GetUserPermissionsAsync(userId, cancellationToken);
        return Ok(permissions);
    }

    /// <summary>
    /// Get specific permission for a user on an option
    /// </summary>
    [HttpGet("users/{userId}/permissions/{optionId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserPermissionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserPermissionDto>> GetUserPermission(Guid userId, string optionId, CancellationToken cancellationToken)
    {
        var permission = await _permissionService.GetUserPermissionAsync(userId, optionId, cancellationToken);
        
        if (permission == null)
            return NotFound(new { error = "Permission not found" });

        return Ok(permission);
    }

    /// <summary>
    /// Assign permission to a user
    /// </summary>
    /// <remarks>
    /// Example:
    /// 
    ///     POST /api/permission/assign
    ///     {
    ///         "userId": "00000000-0000-0000-0000-000000000001",
    ///         "optionId": "facilities",
    ///         "isEnabled": true,
    ///         "permissions": 15  // All = Create | Read | Update | Delete
    ///     }
    /// </remarks>
    [HttpPost("assign")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AssignPermission([FromBody] AssignPermissionDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _permissionService.AssignPermissionAsync(dto, cancellationToken);
        
        if (!success)
            return BadRequest(new { error = "Failed to assign permission" });

        return Ok(new { message = "Permission assigned successfully" });
    }

    /// <summary>
    /// Update permission for a user
    /// </summary>
    [HttpPut("users/{userId}/permissions/{optionId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> UpdatePermission(Guid userId, string optionId, [FromBody] AssignPermissionDto dto, CancellationToken cancellationToken)
    {
        var success = await _permissionService.UpdatePermissionAsync(userId, optionId, dto, cancellationToken);
        
        if (!success)
            return NotFound(new { error = "Permission not found" });

        return Ok(new { message = "Permission updated successfully" });
    }

    /// <summary>
    /// Revoke permission from a user
    /// </summary>
    [HttpDelete("users/{userId}/permissions/{optionId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> RevokePermission(Guid userId, string optionId, CancellationToken cancellationToken)
    {
        var success = await _permissionService.RevokePermissionAsync(userId, optionId, cancellationToken);
        
        if (!success)
            return NotFound(new { error = "Permission not found" });

        return Ok(new { message = "Permission revoked successfully" });
    }

    /// <summary>
    /// Check if user has specific permission
    /// </summary>
    [HttpGet("check")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<ActionResult<bool>> CheckPermission(
        [FromQuery] string optionId,
        [FromQuery] PermissionFlags permission,
        CancellationToken cancellationToken)
    {
        var hasPermission = await _permissionService.CurrentUserHasPermissionAsync(optionId, permission, cancellationToken);
        return Ok(new { hasPermission });
    }
}

