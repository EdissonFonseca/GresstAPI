using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MaterialController : ControllerBase
{
    private readonly IMaterialService _materialService;

    public MaterialController(IMaterialService materialService)
    {
        _materialService = materialService;
    }

    /// <summary>
    /// GET: Obtener todos los materiales
    /// </summary>
    /// <remarks>
    /// Devuelve todos los materiales activos a los que el usuario actual tiene acceso.
    /// Los materiales públicos son visibles para todos, los privados solo para su creador.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), 200)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAll(CancellationToken cancellationToken)
    {
        var materials = await _materialService.GetAllAsync(cancellationToken);
        return Ok(materials);
    }

    /// <summary>
    /// GET: Obtener material por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MaterialDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MaterialDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var material = await _materialService.GetByIdAsync(id, cancellationToken);
        
        if (material == null)
            return NotFound(new { message = "Material not found or you don't have access" });

        return Ok(material);
    }

    /// <summary>
    /// GET: Obtener materiales por tipo de residuo
    /// </summary>
    [HttpGet("wastetype/{wasteTypeId}")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), 200)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetByWasteType(Guid wasteTypeId, CancellationToken cancellationToken)
    {
        var materials = await _materialService.GetByWasteTypeAsync(wasteTypeId, cancellationToken);
        return Ok(materials);
    }

    /// <summary>
    /// GET: Obtener materiales por categoría
    /// </summary>
    /// <param name="category">Categoría: Metal, Plastic, Glass, Organic, etc.</param>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), 200)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetByCategory(string category, CancellationToken cancellationToken)
    {
        var materials = await _materialService.GetByCategoryAsync(category, cancellationToken);
        return Ok(materials);
    }

    /// <summary>
    /// POST: Crear nuevo material
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/material
    ///     {
    ///         "code": "MAT-001",
    ///         "name": "Plástico PET",
    ///         "description": "Polietileno tereftalato reciclable",
    ///         "wasteTypeId": "guid-tipo-residuo",
    ///         "isRecyclable": true,
    ///         "isHazardous": false,
    ///         "category": "Plastic"
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(MaterialDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<MaterialDto>> Create([FromBody] CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var material = await _materialService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = material.Id }, material);
    }

    /// <summary>
    /// PUT: Actualizar material existente
    /// </summary>
    /// <remarks>
    /// Solo se actualizan los campos proporcionados (PATCH-like).
    /// 
    /// Ejemplo:
    /// 
    ///     PUT /api/material/{id}
    ///     {
    ///         "id": "guid-material",
    ///         "name": "Plástico PET Actualizado",
    ///         "isHazardous": true
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MaterialDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MaterialDto>> Update(Guid id, [FromBody] UpdateMaterialDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var material = await _materialService.UpdateAsync(dto, cancellationToken);
        
        if (material == null)
            return NotFound(new { message = "Material not found or you don't have access" });

        return Ok(material);
    }

    /// <summary>
    /// DELETE: Eliminar material (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _materialService.DeleteAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "Material not found or you don't have access" });

        return NoContent();
    }
}

