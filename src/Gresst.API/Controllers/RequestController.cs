using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

/// <summary>
/// Controller for managing requests (Solicitudes)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class RequestController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestService? _requestService;

    public RequestController(
        ICurrentUserService currentUserService,
        IRequestService? requestService = null)
    {
        _currentUserService = currentUserService;
        _requestService = requestService;
    }

    /// <summary>
    /// GET: Get mobile transport waste data for the current user's person
    /// </summary>
    /// <remarks>
    /// Returns mobile transport waste data using the fnResiduosTransporteMovil function.
    /// Uses the current authenticated user's person ID.
    /// </remarks>
    [HttpGet("mobile-transport-waste")]
    [ProducesResponseType(typeof(IEnumerable<MobileTransportWasteDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<IEnumerable<MobileTransportWasteDto>>> GetMobileTransportWaste(
        CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        var personId = _currentUserService.GetCurrentPersonId();
        
        if (personId == Guid.Empty)
        {
            return BadRequest(new { message = "Person ID not found for current user" });
        }

        var results = await _requestService.GetMobileTransportWasteAsync(personId, cancellationToken);
        return Ok(results);
    }

    /// <summary>
    /// GET: Get mobile transport waste data for a specific person
    /// </summary>
    /// <remarks>
    /// Returns mobile transport waste data using the fnResiduosTransporteMovil function.
    /// Allows specifying a person ID to get data for a different person (if authorized).
    /// </remarks>
    /// <param name="personId">Person ID to get data for</param>
    [HttpGet("mobile-transport-waste/{personId}")]
    [ProducesResponseType(typeof(IEnumerable<MobileTransportWasteDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<IEnumerable<MobileTransportWasteDto>>> GetMobileTransportWasteByPersonId(
        Guid personId,
        CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        if (personId == Guid.Empty)
        {
            return BadRequest(new { message = "Invalid person ID" });
        }

        var results = await _requestService.GetMobileTransportWasteAsync(personId, cancellationToken);
        return Ok(results);
    }

    // ==================== CRUD Operations ====================

    /// <summary>
    /// GET: Get all requests - Default for account person (with segmentation)
    /// </summary>
    /// <remarks>
    /// Returns all requests that the current user has access to (segmentation).
    /// By default shows requests for the account person.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RequestDto>), 200)]
    public async Task<ActionResult<IEnumerable<RequestDto>>> GetAll(CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        var requests = await _requestService.GetAllAsync(cancellationToken);
        return Ok(requests);
    }

    /// <summary>
    /// GET: Get request by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RequestDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RequestDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        var request = await _requestService.GetByIdAsync(id.ToString(), cancellationToken);
        
        if (request == null)
            return NotFound(new { message = "Request not found or you don't have access" });

        return Ok(request);
    }

    /// <summary>
    /// GET: Get requests by requester (client)
    /// </summary>
    [HttpGet("requester/{requesterId}")]
    [ProducesResponseType(typeof(IEnumerable<RequestDto>), 200)]
    public async Task<ActionResult<IEnumerable<RequestDto>>> GetByRequester(Guid requesterId, CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        var requests = await _requestService.GetByRequesterAsync(requesterId, cancellationToken);
        return Ok(requests);
    }

    /// <summary>
    /// GET: Get requests by provider
    /// </summary>
    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(typeof(IEnumerable<RequestDto>), 200)]
    public async Task<ActionResult<IEnumerable<RequestDto>>> GetByProvider(Guid providerId, CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        var requests = await _requestService.GetByProviderAsync(providerId, cancellationToken);
        return Ok(requests);
    }

    /// <summary>
    /// GET: Get requests by status
    /// </summary>
    /// <param name="status">Status: Pending, Approved, Rejected, InProgress, Completed, Cancelled</param>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<RequestDto>), 200)]
    public async Task<ActionResult<IEnumerable<RequestDto>>> GetByStatus(string status, CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        var requests = await _requestService.GetByStatusAsync(status, cancellationToken);
        return Ok(requests);
    }

    /// <summary>
    /// POST: Create new request - Default for account person
    /// </summary>
    /// <remarks>
    /// If RequesterId is not specified, uses the account person.
    /// 
    /// Example request:
    /// 
    ///     POST /api/request
    ///     {
    ///         "requesterId": "guid-requester",
    ///         "providerId": "guid-provider",
    ///         "title": "Waste Collection Request",
    ///         "description": "Need to collect waste from facility",
    ///         "servicesRequested": ["Collection", "Transport"],
    ///         "requiredByDate": "2024-12-31T00:00:00Z",
    ///         "pickupAddress": "Calle 100 #50-20",
    ///         "deliveryAddress": "Calle 200 #30-10",
    ///         "items": [
    ///             {
    ///                 "wasteClassId": "guid-waste-class",
    ///                 "estimatedQuantity": 1000,
    ///                 "unit": "kg",
    ///                 "description": "Organic waste"
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(RequestDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<RequestDto>> Create([FromBody] CreateRequestDto dto, CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // If RequesterId is not specified, use account person
        if (dto.RequesterId == Guid.Empty)
        {
            var accountPersonId = _currentUserService.GetCurrentAccountPersonId();
            if (accountPersonId == Guid.Empty)
                return BadRequest(new { message = "RequesterId is required or account person not found" });
            
            dto.RequesterId = accountPersonId;
        }

        var request = await _requestService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }

    /// <summary>
    /// PUT: Update existing request
    /// </summary>
    /// <remarks>
    /// Only provided fields are updated (PATCH-like).
    /// 
    /// Example:
    /// 
    ///     PUT /api/request/{id}
    ///     {
    ///         "id": "guid-request",
    ///         "title": "Updated Title",
    ///         "description": "Updated description",
    ///         "requiredByDate": "2024-12-31T00:00:00Z",
    ///         "estimatedCost": 5000.00
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RequestDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RequestDto>> Update(Guid id, [FromBody] UpdateRequestDto dto, CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        if (id.ToString() != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var request = await _requestService.UpdateAsync(dto, cancellationToken);
        
        if (request == null)
            return NotFound(new { message = "Request not found or you don't have access" });

        return Ok(request);
    }

    /// <summary>
    /// POST: Approve request
    /// </summary>
    /// <remarks>
    /// Approves a pending request and optionally sets the agreed cost.
    /// 
    /// Example:
    /// 
    ///     POST /api/request/{id}/approve
    ///     {
    ///         "agreedCost": 4500.00
    ///     }
    /// </remarks>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(RequestDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RequestDto>> Approve(Guid id, [FromBody] ApproveRequestDto? dto = null, CancellationToken cancellationToken = default)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        var request = await _requestService.ApproveAsync(id, dto?.AgreedCost, cancellationToken);
        
        if (request == null)
            return NotFound(new { message = "Request not found or you don't have access" });

        return Ok(request);
    }

    /// <summary>
    /// POST: Reject request
    /// </summary>
    /// <remarks>
    /// Rejects a pending request with an optional reason.
    /// 
    /// Example:
    /// 
    ///     POST /api/request/{id}/reject
    ///     {
    ///         "reason": "Insufficient capacity"
    ///     }
    /// </remarks>
    [HttpPost("{id}/reject")]
    [ProducesResponseType(typeof(RequestDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RequestDto>> Reject(Guid id, [FromBody] RejectRequestDto dto, CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        if (string.IsNullOrWhiteSpace(dto.Reason))
            return BadRequest(new { message = "Reason is required" });

        var request = await _requestService.RejectAsync(id, dto.Reason, cancellationToken);
        
        if (request == null)
            return NotFound(new { message = "Request not found or you don't have access" });

        return Ok(request);
    }

    /// <summary>
    /// DELETE: Cancel request (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        if (_requestService == null)
            return StatusCode(503, new { message = "Request service is not available" });

        await _requestService.CancelAsync(id, cancellationToken);
        return NoContent();
    }
}

