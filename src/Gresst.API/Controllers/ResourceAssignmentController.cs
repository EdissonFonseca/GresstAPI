using Gresst.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

/// <summary>
/// Controller for assigning/revoking resources to users
/// Manages data segmentation (UsuarioDeposito, UsuarioVehiculo, etc.)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin")]
public class ResourceAssignmentController : ControllerBase
{
    private readonly IDataSegmentationService _segmentationService;

    public ResourceAssignmentController(IDataSegmentationService segmentationService)
    {
        _segmentationService = segmentationService;
    }

    // ==================== FACILITIES ====================

    /// <summary>
    /// Get facilities assigned to a user
    /// </summary>
    [HttpGet("users/{userId}/facilities")]
    [ProducesResponseType(typeof(IEnumerable<Guid>), 200)]
    public async Task<ActionResult<IEnumerable<Guid>>> GetUserFacilities(Guid userId, CancellationToken cancellationToken)
    {
        var facilityIds = await _segmentationService.GetUserFacilityIdsAsync(cancellationToken);
        return Ok(facilityIds);
    }

    /// <summary>
    /// Assign facility to user
    /// </summary>
    /// <remarks>
    /// Allows user to access the facility in GetAll/GetById queries
    /// 
    ///     POST /api/resourceassignment/users/{userId}/facilities/{facilityId}
    /// </remarks>
    [HttpPost("users/{userId}/facilities/{facilityId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AssignFacilityToUser(Guid userId, Guid facilityId, CancellationToken cancellationToken)
    {
        var success = await _segmentationService.AssignFacilityToUserAsync(userId, facilityId, cancellationToken);
        
        if (!success)
            return BadRequest(new { error = "Assignment already exists or failed" });

        return Ok(new { message = "Facility assigned to user successfully" });
    }

    /// <summary>
    /// Revoke facility from user
    /// </summary>
    [HttpDelete("users/{userId}/facilities/{facilityId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> RevokeFacilityFromUser(Guid userId, Guid facilityId, CancellationToken cancellationToken)
    {
        var success = await _segmentationService.RevokeFacilityFromUserAsync(userId, facilityId, cancellationToken);
        
        if (!success)
            return NotFound(new { error = "Assignment not found" });

        return Ok(new { message = "Facility revoked from user successfully" });
    }

    /// <summary>
    /// Check if user has access to facility
    /// </summary>
    [HttpGet("users/{userId}/facilities/{facilityId}/check")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<ActionResult<bool>> CheckFacilityAccess(Guid userId, Guid facilityId, CancellationToken cancellationToken)
    {
        var hasAccess = await _segmentationService.UserHasAccessToFacilityAsync(facilityId, cancellationToken);
        return Ok(new { hasAccess });
    }

    // ==================== VEHICLES ====================

    /// <summary>
    /// Get vehicles assigned to a user
    /// </summary>
    [HttpGet("users/{userId}/vehicles")]
    [ProducesResponseType(typeof(IEnumerable<Guid>), 200)]
    public async Task<ActionResult<IEnumerable<Guid>>> GetUserVehicles(Guid userId, CancellationToken cancellationToken)
    {
        var vehicleIds = await _segmentationService.GetUserVehicleIdsAsync(cancellationToken);
        return Ok(vehicleIds);
    }

    /// <summary>
    /// Assign vehicle to user
    /// </summary>
    [HttpPost("users/{userId}/vehicles/{vehicleId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> AssignVehicleToUser(Guid userId, Guid vehicleId, CancellationToken cancellationToken)
    {
        var success = await _segmentationService.AssignVehicleToUserAsync(userId, vehicleId, cancellationToken);
        
        if (!success)
            return BadRequest(new { error = "Assignment already exists or failed" });

        return Ok(new { message = "Vehicle assigned to user successfully" });
    }

    /// <summary>
    /// Revoke vehicle from user
    /// </summary>
    [HttpDelete("users/{userId}/vehicles/{vehicleId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> RevokeVehicleFromUser(Guid userId, Guid vehicleId, CancellationToken cancellationToken)
    {
        var success = await _segmentationService.RevokeVehicleFromUserAsync(userId, vehicleId, cancellationToken);
        
        if (!success)
            return NotFound(new { error = "Assignment not found" });

        return Ok(new { message = "Vehicle revoked from user successfully" });
    }

    /// <summary>
    /// Check if user has access to vehicle
    /// </summary>
    [HttpGet("users/{userId}/vehicles/{vehicleId}/check")]
    [ProducesResponseType(typeof(bool), 200)]
    public async Task<ActionResult<bool>> CheckVehicleAccess(Guid userId, Guid vehicleId, CancellationToken cancellationToken)
    {
        var hasAccess = await _segmentationService.UserHasAccessToVehicleAsync(vehicleId, cancellationToken);
        return Ok(new { hasAccess });
    }
}

