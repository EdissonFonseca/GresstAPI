using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

/// <summary>
/// Controller for managing processes, subprocesses, and tasks
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ProcessController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestService? _requestService;
    private readonly IProcessService? _processService;

    public ProcessController(
        ICurrentUserService currentUserService,
        IRequestService? requestService = null,
        IProcessService? processService = null)
    {
        _currentUserService = currentUserService;
        _requestService = requestService;
        _processService = processService;
    }

    /// <summary>
    /// Get transport operations as processes with subprocesses and tasks
    /// </summary>
    /// <remarks>
    /// Transport hierarchy:
    /// - Process  : Service Order (Orden de servicio) associated to a vehicle (IdOrden / IdVehiculo)
    /// - SubProcess: Each collection point for the order (OrdenPlaneacion → grouped by request + origin depot)
    /// - Task    : Each request detail (SolicitudDetalle) at that collection point (\"pick up this material\")
    /// </remarks>
    [HttpGet("transport-requests")]
    [ProducesResponseType(typeof(IEnumerable<ProcessDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<IEnumerable<ProcessDto>>> GetTransportRequests(
        CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        if (_processService == null)
            return StatusCode(503, new { message = "Process service is not available" });

        var personId = _currentUserService.GetCurrentPersonId();
        
        if (personId == Guid.Empty)
        {
            return BadRequest(new { message = "Person ID not found for current user" });
        }

        var transportData = await _requestService.GetMobileTransportWasteAsync(personId, cancellationToken);
        var processes = await _processService.MapTransportDataToProcessesAsync(transportData, cancellationToken);
        
        return Ok(processes);
    }
}

