using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManagementController : ControllerBase
{
    private readonly IManagementService _managementService;

    public ManagementController(IManagementService managementService)
    {
        _managementService = managementService;
    }

    /// <summary>
    /// Get waste history (all management operations)
    /// </summary>
    [HttpGet("waste/{wasteId}/history")]
    public async Task<ActionResult<IEnumerable<ManagementDto>>> GetWasteHistory(Guid wasteId, CancellationToken cancellationToken)
    {
        var history = await _managementService.GetWasteHistoryAsync(wasteId, cancellationToken);
        return Ok(history);
    }

    /// <summary>
    /// Create generic management operation
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ManagementDto>> Create([FromBody] CreateManagementDto dto, CancellationToken cancellationToken)
    {
        var management = await _managementService.CreateManagementAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetWasteHistory), new { wasteId = dto.WasteId }, management);
    }

    /// <summary>
    /// Operation 1: Generate waste
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<ManagementDto>> Generate([FromBody] CreateWasteDto dto, CancellationToken cancellationToken)
    {
        var management = await _managementService.GenerateWasteAsync(dto, cancellationToken);
        return Ok(management);
    }

    /// <summary>
    /// Operation 2: Collect waste
    /// </summary>
    [HttpPost("collect")]
    public async Task<ActionResult<ManagementDto>> Collect([FromBody] CollectWasteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var management = await _managementService.CollectWasteAsync(dto, cancellationToken);
            return Ok(management);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Operation 3: Transport waste
    /// </summary>
    [HttpPost("transport")]
    public async Task<ActionResult<ManagementDto>> Transport([FromBody] TransportWasteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var management = await _managementService.TransportWasteAsync(dto, cancellationToken);
            return Ok(management);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Operation 4: Receive waste
    /// </summary>
    [HttpPost("receive")]
    public async Task<ActionResult<ManagementDto>> Receive([FromBody] ReceiveWasteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var management = await _managementService.ReceiveWasteAsync(
                dto.WasteId, dto.ReceiverId, dto.FacilityId, cancellationToken);
            return Ok(management);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Operation 5: Transform waste (Convert, Decompose, Group)
    /// </summary>
    [HttpPost("transform")]
    public async Task<ActionResult<ManagementDto>> Transform([FromBody] TransformWasteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var management = await _managementService.TransformWasteAsync(dto, cancellationToken);
            return Ok(management);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Operation 6 & 7: Store waste (temporary or permanent)
    /// </summary>
    [HttpPost("store")]
    public async Task<ActionResult<ManagementDto>> Store([FromBody] StoreWasteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var management = await _managementService.StoreWasteAsync(dto, cancellationToken);
            return Ok(management);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Operation 8: Sell waste
    /// </summary>
    [HttpPost("sell")]
    public async Task<ActionResult<ManagementDto>> Sell([FromBody] SellWasteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var management = await _managementService.SellWasteAsync(
                dto.WasteId, dto.BuyerId, dto.Price, cancellationToken);
            return Ok(management);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Operation 9: Deliver to third party
    /// </summary>
    [HttpPost("deliver")]
    public async Task<ActionResult<ManagementDto>> Deliver([FromBody] DeliverWasteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var management = await _managementService.DeliverToThirdPartyAsync(
                dto.WasteId, dto.RecipientId, dto.Notes ?? "", cancellationToken);
            return Ok(management);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Operation 10: Dispose waste
    /// </summary>
    [HttpPost("dispose")]
    public async Task<ActionResult<ManagementDto>> Dispose([FromBody] DisposeWasteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var management = await _managementService.DisposeWasteAsync(dto, cancellationToken);
            return Ok(management);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Operation 11: Classify waste
    /// </summary>
    [HttpPost("classify")]
    public async Task<ActionResult<ManagementDto>> Classify([FromBody] ClassifyWasteDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var management = await _managementService.ClassifyWasteAsync(
                dto.WasteId, dto.WasteTypeId, dto.ClassifiedById, cancellationToken);
            return Ok(management);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

// Additional DTOs for operations
public class ReceiveWasteDto
{
    public Guid WasteId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid FacilityId { get; set; }
}

public class SellWasteDto
{
    public Guid WasteId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal Price { get; set; }
}

public class DeliverWasteDto
{
    public Guid WasteId { get; set; }
    public Guid RecipientId { get; set; }
    public string? Notes { get; set; }
}

public class ClassifyWasteDto
{
    public Guid WasteId { get; set; }
    public Guid WasteTypeId { get; set; }
    public Guid ClassifiedById { get; set; }
}

