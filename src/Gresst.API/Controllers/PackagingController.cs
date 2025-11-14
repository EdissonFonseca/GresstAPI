using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PackagingController : ControllerBase
{
    private readonly IPackagingService _packagingService;

    public PackagingController(IPackagingService packagingService)
    {
        _packagingService = packagingService;
    }

    /// <summary>
    /// GET: Obtener todos los embalajes
    /// </summary>
    /// <remarks>
    /// Devuelve todos los embalajes activos del sistema.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PackagingDto>), 200)]
    public async Task<ActionResult<IEnumerable<PackagingDto>>> GetAll(CancellationToken cancellationToken)
    {
        var packagings = await _packagingService.GetAllAsync(cancellationToken);
        return Ok(packagings);
    }

    /// <summary>
    /// GET: Obtener embalaje por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PackagingDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PackagingDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var packaging = await _packagingService.GetByIdAsync(id, cancellationToken);
        
        if (packaging == null)
            return NotFound(new { message = "Packaging not found" });

        return Ok(packaging);
    }

    /// <summary>
    /// GET: Obtener embalajes por tipo
    /// </summary>
    /// <param name="packagingType">Tipo de embalaje (Drum, Bag, Container, Tank, etc.)</param>
    [HttpGet("type/{packagingType}")]
    [ProducesResponseType(typeof(IEnumerable<PackagingDto>), 200)]
    public async Task<ActionResult<IEnumerable<PackagingDto>>> GetByType(string packagingType, CancellationToken cancellationToken)
    {
        var packagings = await _packagingService.GetByTypeAsync(packagingType, cancellationToken);
        return Ok(packagings);
    }

    /// <summary>
    /// POST: Crear nuevo embalaje
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/packaging
    ///     {
    ///         "code": "DRUM-001",
    ///         "name": "Tambor de 200L",
    ///         "description": "Tambor metálico reutilizable de 200 litros",
    ///         "packagingType": "Drum",
    ///         "capacity": 200,
    ///         "capacityUnit": "L",
    ///         "isReusable": true,
    ///         "material": "Steel",
    ///         "unPackagingCode": "1A1",
    ///         "parentPackagingId": null
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(PackagingDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PackagingDto>> Create([FromBody] CreatePackagingDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var packaging = await _packagingService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = packaging.Id }, packaging);
    }

    /// <summary>
    /// PUT: Actualizar embalaje existente
    /// </summary>
    /// <remarks>
    /// Solo se actualizan los campos proporcionados (PATCH-like).
    /// Para limpiar el parent packaging, enviar parentPackagingId como Guid.Empty.
    /// 
    /// Ejemplo:
    /// 
    ///     PUT /api/packaging/{id}
    ///     {
    ///         "id": "guid-packaging",
    ///         "name": "Tambor Actualizado",
    ///         "capacity": 250,
    ///         "isReusable": false,
    ///         "parentPackagingId": "guid-nuevo-parent"
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PackagingDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PackagingDto>> Update(Guid id, [FromBody] UpdatePackagingDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var packaging = await _packagingService.UpdateAsync(dto, cancellationToken);
        
        if (packaging == null)
            return NotFound(new { message = "Packaging not found" });

        return Ok(packaging);
    }

    /// <summary>
    /// DELETE: Eliminar embalaje (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _packagingService.DeleteAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "Packaging not found" });

        return NoContent();
    }
}

