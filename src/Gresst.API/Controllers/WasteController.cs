using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WasteController : ControllerBase
{
    private readonly IWasteService _wasteService;

    public WasteController(IWasteService wasteService)
    {
        _wasteService = wasteService;
    }

    /// <summary>
    /// Get all wastes
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WasteDto>>> GetAll(CancellationToken cancellationToken)
    {
        var wastes = await _wasteService.GetAllAsync(cancellationToken);
        return Ok(wastes);
    }

    /// <summary>
    /// Get waste by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<WasteDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var waste = await _wasteService.GetByIdAsync(id, cancellationToken);
            return Ok(waste);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Get wastes by generator
    /// </summary>
    [HttpGet("generator/{generatorId}")]
    public async Task<ActionResult<IEnumerable<WasteDto>>> GetByGenerator(Guid generatorId, CancellationToken cancellationToken)
    {
        var wastes = await _wasteService.GetByGeneratorAsync(generatorId, cancellationToken);
        return Ok(wastes);
    }

    /// <summary>
    /// Get wastes by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<WasteDto>>> GetByStatus(string status, CancellationToken cancellationToken)
    {
        try
        {
            var wastes = await _wasteService.GetByStatusAsync(status, cancellationToken);
            return Ok(wastes);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get waste bank - wastes available for reuse
    /// </summary>
    [HttpGet("bank")]
    public async Task<ActionResult<IEnumerable<WasteDto>>> GetWasteBank(CancellationToken cancellationToken)
    {
        var wastes = await _wasteService.GetWasteBankAsync(cancellationToken);
        return Ok(wastes);
    }

    /// <summary>
    /// Create new waste (Generate operation)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WasteDto>> Create([FromBody] CreateWasteDto dto, CancellationToken cancellationToken)
    {
        var waste = await _wasteService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = waste.Id }, waste);
    }

    /// <summary>
    /// Update waste
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<WasteDto>> Update(Guid id, [FromBody] UpdateWasteDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var waste = await _wasteService.UpdateAsync(dto, cancellationToken);
            return Ok(waste);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Delete waste
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _wasteService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Publish waste to waste bank
    /// </summary>
    [HttpPost("{id}/publish-to-bank")]
    public async Task<ActionResult> PublishToBank(Guid id, [FromBody] PublishToBankDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _wasteService.PublishToWasteBankAsync(id, dto.Description, dto.Price, cancellationToken);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Remove waste from waste bank
    /// </summary>
    [HttpPost("{id}/remove-from-bank")]
    public async Task<ActionResult> RemoveFromBank(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _wasteService.RemoveFromWasteBankAsync(id, cancellationToken);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

public class PublishToBankDto
{
    public string Description { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}

