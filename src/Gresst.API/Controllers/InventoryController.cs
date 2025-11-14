using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IBalanceService _balanceService;

    public InventoryController(IBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    /// <summary>
    /// Get inventory balances with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BalanceDto>>> GetInventory(
        [FromQuery] Guid? personId,
        [FromQuery] Guid? facilityId,
        [FromQuery] Guid? locationId,
        [FromQuery] Guid? wasteTypeId,
        CancellationToken cancellationToken)
    {
        var query = new InventoryQueryDto
        {
            PersonId = personId,
            FacilityId = facilityId,
            LocationId = locationId,
            WasteClassId = wasteTypeId
        };

        var inventory = await _balanceService.GetInventoryAsync(query, cancellationToken);
        return Ok(inventory);
    }

    /// <summary>
    /// Get specific balance
    /// </summary>
    [HttpGet("balance")]
    public async Task<ActionResult<BalanceDto>> GetBalance(
        [FromQuery] Guid? personId,
        [FromQuery] Guid? facilityId,
        [FromQuery] Guid? locationId,
        [FromQuery] Guid wasteTypeId,
        CancellationToken cancellationToken)
    {
        var balance = await _balanceService.GetBalanceAsync(
            personId, facilityId, locationId, wasteTypeId, cancellationToken);
        
        if (balance == null)
            return NotFound("Balance not found");

        return Ok(balance);
    }
}

