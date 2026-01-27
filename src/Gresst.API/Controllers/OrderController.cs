using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

/// <summary>
/// Controller for managing service orders (Órdenes de servicio)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrderService? _orderService;

    public OrderController(
        ICurrentUserService currentUserService,
        IOrderService? orderService = null)
    {
        _currentUserService = currentUserService;
        _orderService = orderService;
    }

    // ==================== CRUD Operations ====================

    /// <summary>
    /// GET: Get all orders - Default for account person (with segmentation)
    /// </summary>
    /// <remarks>
    /// Returns all orders that the current user has access to (segmentation).
    /// By default shows orders for the account person.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        var orders = await _orderService.GetAllAsync(cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// GET: Get order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        var order = await _orderService.GetByIdAsync(id.ToString(), cancellationToken);
        
        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(order);
    }

    /// <summary>
    /// POST: Create a new order
    /// </summary>
    /// <param name="dto">Order creation data</param>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<OrderDto>> Create(
        [FromBody] CreateOrderDto dto,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var order = await _orderService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// PUT: Update an existing order
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="dto">Order update data</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<OrderDto>> Update(
        Guid id,
        [FromBody] CreateOrderDto dto,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Create update DTO with ID
        var updateDto = new { Id = id, dto.Type, dto.ProviderId, dto.ClientId, dto.RequestId, dto.ScheduledDate, dto.Description, dto.EstimatedCost, dto.VehicleId, dto.FacilityId, dto.RouteId, dto.Items };
        
        var order = await _orderService.UpdateAsync(updateDto, cancellationToken);
        
        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(order);
    }

    // ==================== Query Methods ====================

    /// <summary>
    /// GET: Get orders by provider
    /// </summary>
    /// <param name="providerId">Provider person ID</param>
    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByProvider(
        Guid providerId,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (providerId == Guid.Empty)
            return BadRequest(new { message = "Invalid provider ID" });

        var orders = await _orderService.GetByProviderAsync(providerId, cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// GET: Get orders by client
    /// </summary>
    /// <param name="clientId">Client person ID</param>
    [HttpGet("client/{clientId}")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByClient(
        Guid clientId,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (clientId == Guid.Empty)
            return BadRequest(new { message = "Invalid client ID" });

        var orders = await _orderService.GetByClientAsync(clientId, cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// GET: Get orders by status
    /// </summary>
    /// <param name="status">Order status (Pending, Approved, InProgress, Completed, Cancelled)</param>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(
        string status,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (string.IsNullOrWhiteSpace(status))
            return BadRequest(new { message = "Status is required" });

        var orders = await _orderService.GetByStatusAsync(status, cancellationToken);
        return Ok(orders);
    }

    /// <summary>
    /// GET: Get scheduled orders within a date range
    /// </summary>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    [HttpGet("scheduled")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetScheduled(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (startDate > endDate)
            return BadRequest(new { message = "Start date must be before end date" });

        var orders = await _orderService.GetScheduledAsync(startDate, endDate, cancellationToken);
        return Ok(orders);
    }

    // ==================== Order Lifecycle Actions ====================

    /// <summary>
    /// POST: Schedule an order
    /// </summary>
    /// <remarks>
    /// Schedules an order for a specific date and optionally assigns a vehicle and route.
    /// </remarks>
    /// <param name="id">Order ID</param>
    /// <param name="scheduledDate">Date and time to schedule the order</param>
    /// <param name="vehicleId">Optional vehicle ID to assign</param>
    /// <param name="routeId">Optional route ID to assign</param>
    [HttpPost("{id}/schedule")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<OrderDto>> Schedule(
        Guid id,
        [FromBody] ScheduleOrderDto dto,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (id == Guid.Empty)
            return BadRequest(new { message = "Invalid order ID" });

        var order = await _orderService.ScheduleAsync(
            id, 
            dto.ScheduledDate, 
            dto.VehicleId, 
            dto.RouteId, 
            cancellationToken);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(order);
    }

    /// <summary>
    /// POST: Start an order (mark as in progress)
    /// </summary>
    /// <param name="id">Order ID</param>
    [HttpPost("{id}/start")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<OrderDto>> Start(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (id == Guid.Empty)
            return BadRequest(new { message = "Invalid order ID" });

        var order = await _orderService.StartAsync(id, cancellationToken);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(order);
    }

    /// <summary>
    /// POST: Complete an order
    /// </summary>
    /// <remarks>
    /// Marks an order as completed and optionally records the actual cost.
    /// </remarks>
    /// <param name="id">Order ID</param>
    /// <param name="dto">Completion data including actual cost</param>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(OrderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(503)]
    public async Task<ActionResult<OrderDto>> Complete(
        Guid id,
        [FromBody] CompleteOrderDto? dto,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (id == Guid.Empty)
            return BadRequest(new { message = "Invalid order ID" });

        var order = await _orderService.CompleteAsync(
            id, 
            dto?.ActualCost, 
            cancellationToken);

        if (order == null)
            return NotFound(new { message = "Order not found" });

        return Ok(order);
    }

    /// <summary>
    /// POST: Cancel an order
    /// </summary>
    /// <remarks>
    /// Cancels an order and optionally records a cancellation reason.
    /// </remarks>
    /// <param name="id">Order ID</param>
    /// <param name="dto">Cancellation data including reason</param>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(503)]
    public async Task<ActionResult> Cancel(
        Guid id,
        [FromBody] CancelOrderDto? dto,
        CancellationToken cancellationToken)
    {
        if (_orderService == null)
            return StatusCode(503, new { message = "Order service is not available" });

        if (id == Guid.Empty)
            return BadRequest(new { message = "Invalid order ID" });

        await _orderService.CancelAsync(id, dto?.Reason, cancellationToken);
        return Ok(new { message = "Order cancelled successfully" });
    }
}

