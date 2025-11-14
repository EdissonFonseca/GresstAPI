using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProviderController : ControllerBase
{
    private readonly IProviderService _providerService;

    public ProviderController(IProviderService providerService)
    {
        _providerService = providerService;
    }

    /// <summary>
    /// GET: Obtener todos los proveedores
    /// </summary>
    /// <remarks>
    /// Devuelve todos los proveedores activos de la cuenta actual.
    /// Los proveedores son Personas con rol de Proveedor.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProviderDto>), 200)]
    public async Task<ActionResult<IEnumerable<ProviderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var providers = await _providerService.GetAllAsync(cancellationToken);
        return Ok(providers);
    }

    /// <summary>
    /// GET: Obtener proveedor por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProviderDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ProviderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var provider = await _providerService.GetByIdAsync(id, cancellationToken);
        
        if (provider == null)
            return NotFound(new { message = "Provider not found" });

        return Ok(provider);
    }

    /// <summary>
    /// GET: Obtener proveedor por número de documento
    /// </summary>
    [HttpGet("document/{documentNumber}")]
    [ProducesResponseType(typeof(IEnumerable<ProviderDto>), 200)]
    public async Task<ActionResult<IEnumerable<ProviderDto>>> GetByDocumentNumber(string documentNumber, CancellationToken cancellationToken)
    {
        var providers = await _providerService.GetByDocumentNumberAsync(documentNumber, cancellationToken);
        return Ok(providers);
    }

    /// <summary>
    /// POST: Crear nuevo proveedor
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/provider
    ///     {
    ///         "name": "Transportes Proveedor S.A.S.",
    ///         "documentNumber": "900789012-1",
    ///         "email": "contacto@proveedor.com",
    ///         "phone": "+57 300 7890123",
    ///         "address": "Carrera 50 #80-30, Bogotá",
    ///         "isCollector": true,
    ///         "isTransporter": true
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ProviderDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ProviderDto>> Create([FromBody] CreateProviderDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var provider = await _providerService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = provider.Id }, provider);
    }

    /// <summary>
    /// PUT: Actualizar proveedor existente
    /// </summary>
    /// <remarks>
    /// Solo se actualizan los campos proporcionados (PATCH-like).
    /// 
    /// Ejemplo:
    /// 
    ///     PUT /api/provider/{id}
    ///     {
    ///         "id": "guid-provider",
    ///         "name": "Transportes Proveedor Actualizado",
    ///         "email": "nuevo@proveedor.com"
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProviderDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ProviderDto>> Update(Guid id, [FromBody] UpdateProviderDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var provider = await _providerService.UpdateAsync(dto, cancellationToken);
        
        if (provider == null)
            return NotFound(new { message = "Provider not found" });

        return Ok(provider);
    }

    /// <summary>
    /// DELETE: Eliminar proveedor (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _providerService.DeleteAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "Provider not found" });

        return NoContent();
    }
}

