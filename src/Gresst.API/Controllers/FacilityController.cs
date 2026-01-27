using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FacilityController : ControllerBase
{
    private readonly IFacilityService _facilityService;
    private readonly IMaterialService _materialService;

    public FacilityController(IFacilityService facilityService, IMaterialService materialService)
    {
        _facilityService = facilityService;
        _materialService = materialService;
    }

    /// <summary>
    /// GET: Obtener todas las facilities (depósitos) - Por defecto para la persona de la cuenta
    /// </summary>
    /// <remarks>
    /// Devuelve todas las facilities activas de la persona de la cuenta (por defecto).
    /// Las facilities son depósitos, plantas, sitios de disposición, etc.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FacilityDto>), 200)]
    public async Task<ActionResult<IEnumerable<FacilityDto>>> GetAll(CancellationToken cancellationToken)
    {
        var facilities = await _facilityService.GetAccountPersonFacilitiesAsync(cancellationToken);
        return Ok(facilities);
    }

    /// <summary>
    /// GET: Obtener facilities de la persona de la cuenta (explícito)
    /// </summary>
    [HttpGet("account")]
    [ProducesResponseType(typeof(IEnumerable<FacilityDto>), 200)]
    public async Task<ActionResult<IEnumerable<FacilityDto>>> GetAccountPersonFacilities(CancellationToken cancellationToken)
    {
        var facilities = await _facilityService.GetAccountPersonFacilitiesAsync(cancellationToken);
        return Ok(facilities);
    }

    // Note: Facility endpoints for clients and providers have been moved to PersonController
    // Use GET /api/person/{personId}/facility instead

    /// <summary>
    /// GET: Obtener facility por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FacilityDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<FacilityDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var facility = await _facilityService.GetByIdAsync(id.ToString(), cancellationToken);
        
        if (facility == null)
            return NotFound(new { message = "Facility not found or you don't have access" });

        return Ok(facility);
    }

    /// <summary>
    /// GET: Obtener facilities por persona (dueño)
    /// </summary>
    [HttpGet("person/{personId}")]
    [ProducesResponseType(typeof(IEnumerable<FacilityDto>), 200)]
    public async Task<ActionResult<IEnumerable<FacilityDto>>> GetByPerson(Guid personId, CancellationToken cancellationToken)
    {
        var facilities = await _facilityService.GetByPersonAsync(personId, cancellationToken);
        return Ok(facilities);
    }

    /// <summary>
    /// GET: Obtener facilities por tipo
    /// </summary>
    /// <param name="type">Tipo: TreatmentPlant, DisposalSite, StorageFacility, TransferStation</param>
    [HttpGet("type/{type}")]
    [ProducesResponseType(typeof(IEnumerable<FacilityDto>), 200)]
    public async Task<ActionResult<IEnumerable<FacilityDto>>> GetByType(string type, CancellationToken cancellationToken)
    {
        var facilities = await _facilityService.GetByTypeAsync(type, cancellationToken);
        return Ok(facilities);
    }

    /// <summary>
    /// POST: Crear nueva facility (depósito) - Por defecto para la persona de la cuenta
    /// </summary>
    /// <remarks>
    /// Si no se especifica PersonId, se usa la persona de la cuenta.
    /// Las facilities pueden tener una estructura jerárquica usando ParentFacilityId.
    /// 
    /// Ejemplo de request:
    /// 
    ///     POST /api/facility
    ///     {
    ///         "code": "DEP-001",
    ///         "name": "Planta de Tratamiento Norte",
    ///         "description": "Planta principal de tratamiento",
    ///         "facilityType": "TreatmentPlant",
    ///         "address": "Calle 100 #50-20",
    ///         "latitude": 4.701594,
    ///         "longitude": -74.035126,
    ///         "canCollect": true,
    ///         "canStore": true,
    ///         "canTreat": true,
    ///         "maxCapacity": 10000,
    ///         "capacityUnit": "kg",
    ///         "parentFacilityId": "guid-facility-padre", // Opcional: para estructuras jerárquicas
    ///         "isVirtual": false // Opcional: para facilities virtuales (p. ej., vehículos)
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(FacilityDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<FacilityDto>> Create([FromBody] CreateFacilityDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var facility = await _facilityService.CreateAccountPersonFacilityAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = facility.Id }, facility);
    }

    /// <summary>
    /// POST: Crear facility para la persona de la cuenta (explícito)
    /// </summary>
    [HttpPost("account")]
    [ProducesResponseType(typeof(FacilityDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<FacilityDto>> CreateAccountPersonFacility([FromBody] CreateFacilityDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var facility = await _facilityService.CreateAccountPersonFacilityAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = facility.Id }, facility);
    }

    // Note: Facility creation endpoints for clients and providers have been moved to PersonController
    // Use POST /api/person/{personId}/facility instead

    /// <summary>
    /// PUT: Actualizar facility existente
    /// </summary>
    /// <remarks>
    /// Solo se actualizan los campos proporcionados (PATCH-like).
    /// Para limpiar el parent facility, enviar parentFacilityId como Guid.Empty.
    /// 
    /// Ejemplo:
    /// 
    ///     PUT /api/facility/{id}
    ///     {
    ///         "id": "guid-facility",
    ///         "name": "Planta Actualizada",
    ///         "canDispose": true,
    ///         "maxCapacity": 15000,
    ///         "parentFacilityId": "guid-nuevo-parent", // Opcional: actualizar parent
    ///         "isVirtual": false // Opcional: actualizar si es virtual
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FacilityDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<FacilityDto>> Update(Guid id, [FromBody] UpdateFacilityDto dto, CancellationToken cancellationToken)
    {
        if (id.ToString() != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var facility = await _facilityService.UpdateAsync(dto, cancellationToken);
        
        if (facility == null)
            return NotFound(new { message = "Facility not found or you don't have access" });

        return Ok(facility);
    }

    /// <summary>
    /// DELETE: Eliminar facility (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _facilityService.DeleteAsync(id.ToString(), cancellationToken);
        
        if (!success)
            return NotFound(new { message = "Facility not found or you don't have access" });

        return NoContent();
    }

    // Material endpoints - Nested under facility
    /// <summary>
    /// GET: Obtener materiales de un facility
    /// </summary>
    [HttpGet("{facilityId}/material")]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), 200)]
    public async Task<ActionResult<IEnumerable<MaterialDto>>> GetFacilityMaterials(Guid facilityId, CancellationToken cancellationToken)
    {
        var materials = await _materialService.GetFacilityMaterialsAsync(facilityId, cancellationToken);
        return Ok(materials);
    }

    /// <summary>
    /// POST: Crear material para un facility
    /// </summary>
    [HttpPost("{facilityId}/material")]
    [ProducesResponseType(typeof(MaterialDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<MaterialDto>> CreateFacilityMaterial(Guid facilityId, [FromBody] CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var material = await _materialService.CreateFacilityMaterialAsync(facilityId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetFacilityMaterials), new { facilityId }, material);
    }
}

