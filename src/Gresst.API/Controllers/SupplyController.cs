using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class SupplyController : ControllerBase
{
    private readonly ISupplyService _supplyService;

    public SupplyController(ISupplyService supplyService)
    {
        _supplyService = supplyService;
    }

    /// <summary>
    /// GET: Obtener todos los insumos
    /// </summary>
    /// <remarks>
    /// Devuelve todos los insumos activos disponibles para la cuenta actual.
    /// Incluye insumos públicos y privados de la cuenta.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SupplyDto>), 200)]
    public async Task<ActionResult<IEnumerable<SupplyDto>>> GetAll(CancellationToken cancellationToken)
    {
        var supplies = await _supplyService.GetAllAsync(cancellationToken);
        return Ok(supplies);
    }

    /// <summary>
    /// GET: Obtener insumos públicos
    /// </summary>
    /// <remarks>
    /// Devuelve solo los insumos marcados como públicos (visibles para todas las cuentas).
    /// </remarks>
    [HttpGet("public")]
    [ProducesResponseType(typeof(IEnumerable<SupplyDto>), 200)]
    public async Task<ActionResult<IEnumerable<SupplyDto>>> GetPublicSupplies(CancellationToken cancellationToken)
    {
        var supplies = await _supplyService.GetPublicSuppliesAsync(cancellationToken);
        return Ok(supplies);
    }

    /// <summary>
    /// GET: Obtener insumos por categoría/unidad
    /// </summary>
    /// <param name="categoryUnitId">ID de la categoría/unidad</param>
    [HttpGet("category/{categoryUnitId}")]
    [ProducesResponseType(typeof(IEnumerable<SupplyDto>), 200)]
    public async Task<ActionResult<IEnumerable<SupplyDto>>> GetByCategory(string categoryUnitId, CancellationToken cancellationToken)
    {
        var supplies = await _supplyService.GetByCategoryAsync(categoryUnitId, cancellationToken);
        return Ok(supplies);
    }

    /// <summary>
    /// GET: Obtener insumo por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SupplyDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SupplyDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var supply = await _supplyService.GetByIdAsync(id.ToString(), cancellationToken);
        
        if (supply == null)
            return NotFound(new { message = "Supply not found" });

        return Ok(supply);
    }

    /// <summary>
    /// POST: Crear nuevo insumo
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/supply
    ///     {
    ///         "code": "INS-001",
    ///         "name": "Combustible Diesel",
    ///         "description": "Combustible diesel para vehículos",
    ///         "categoryUnitId": "LIT",
    ///         "isPublic": false,
    ///         "parentSupplyId": null
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(SupplyDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<SupplyDto>> Create([FromBody] CreateSupplyDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var supply = await _supplyService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = supply.Id }, supply);
    }

    /// <summary>
    /// PUT: Actualizar insumo existente
    /// </summary>
    /// <remarks>
    /// Solo se actualizan los campos proporcionados (PATCH-like).
    /// Para limpiar el parent supply, enviar parentSupplyId como Guid.Empty.
    /// 
    /// Ejemplo:
    /// 
    ///     PUT /api/supply/{id}
    ///     {
    ///         "id": "guid-supply",
    ///         "name": "Combustible Actualizado",
    ///         "categoryUnitId": "GAL",
    ///         "isPublic": true,
    ///         "parentSupplyId": "guid-nuevo-parent"
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SupplyDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SupplyDto>> Update(Guid id, [FromBody] UpdateSupplyDto dto, CancellationToken cancellationToken)
    {
        if (id.ToString() != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var supply = await _supplyService.UpdateAsync(dto, cancellationToken);
        
        if (supply == null)
            return NotFound(new { message = "Supply not found" });

        return Ok(supply);
    }

    /// <summary>
    /// DELETE: Eliminar insumo (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _supplyService.DeleteAsync(id.ToString(), cancellationToken);
        
        if (!success)
            return NotFound(new { message = "Supply not found" });

        return NoContent();
    }
}

