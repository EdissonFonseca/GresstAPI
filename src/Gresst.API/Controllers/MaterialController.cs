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
    /// GET: Obtener todos los materiales - Por defecto para la persona de la cuenta
    /// </summary>
    /// <remarks>
    /// Devuelve todos los materiales activos de la persona de la cuenta (por defecto).
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), 200)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAll(CancellationToken cancellationToken)
    {
        var materials = await _materialService.GetAccountPersonMaterialsAsync(cancellationToken);
        return Ok(materials);
    }

    /// <summary>
    /// GET: Obtener materiales de la persona de la cuenta (explícito)
    /// </summary>
    [HttpGet("account")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), 200)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetAccountPersonMaterials(CancellationToken cancellationToken)
    {
        var materials = await _materialService.GetAccountPersonMaterialsAsync(cancellationToken);
        return Ok(materials);
    }

    // Note: Material endpoints for clients and providers have been moved to PersonController
    // Use GET /api/person/{personId}/material instead

    // Note: Facility material endpoints have been moved to FacilityController
    // Use GET /api/facility/{facilityId}/material instead

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
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetByWasteClass(Guid wasteTypeId, CancellationToken cancellationToken)
    {
        var materials = await _materialService.GetByWasteClassAsync(wasteTypeId, cancellationToken);
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
    /// POST: Crear nuevo material - Por defecto para la persona de la cuenta
    /// </summary>
    /// <remarks>
    /// Si no se especifica, se crea para la persona de la cuenta.
    /// 
    /// Ejemplo de request:
    /// 
    ///     POST /api/material
    ///     {
    ///         "code": "MAT-001",
    ///         "name": "Plástico PET",
    ///         "description": "Polietileno tereftalato reciclable",
    ///         "wasteClassId": "guid-tipo-residuo",
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

        var material = await _materialService.CreateAccountPersonMaterialAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = material.Id }, material);
    }

    /// <summary>
    /// POST: Crear material para la persona de la cuenta (explícito)
    /// </summary>
    [HttpPost("account")]
    [ProducesResponseType(typeof(MaterialDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<MaterialDto>> CreateAccountPersonMaterial([FromBody] CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var material = await _materialService.CreateAccountPersonMaterialAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = material.Id }, material);
    }

    // Note: Material creation endpoints for clients and providers have been moved to PersonController
    // Use POST /api/person/{personId}/material instead

    // Note: Facility material creation endpoints have been moved to FacilityController
    // Use POST /api/facility/{facilityId}/material instead

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

