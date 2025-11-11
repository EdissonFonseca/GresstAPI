using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacilityController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public FacilityController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    /// <summary>
    /// GET: Obtener todas las facilities (depósitos)
    /// </summary>
    /// <remarks>
    /// Devuelve todas las facilities activas de la cuenta actual.
    /// Las facilities son depósitos, plantas, sitios de disposición, etc.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FacilityDto>), 200)]
    public async Task<ActionResult<IEnumerable<FacilityDto>>> GetAll(CancellationToken cancellationToken)
    {
        var facilities = await _facilityService.GetAllAsync(cancellationToken);
        return Ok(facilities);
    }

    /// <summary>
    /// GET: Obtener facility por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FacilityDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<FacilityDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var facility = await _facilityService.GetByIdAsync(id, cancellationToken);
            return Ok(facility);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
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
    /// POST: Crear nueva facility (depósito)
    /// </summary>
    /// <remarks>
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
    ///         "personId": "guid-persona",
    ///         "canCollect": true,
    ///         "canStore": true,
    ///         "canTreat": true,
    ///         "maxCapacity": 10000,
    ///         "capacityUnit": "kg"
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(FacilityDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<FacilityDto>> Create([FromBody] CreateFacilityDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var facility = await _facilityService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = facility.Id }, facility);
    }

    /// <summary>
    /// PUT: Actualizar facility existente
    /// </summary>
    /// <remarks>
    /// Solo se actualizan los campos proporcionados (PATCH-like).
    /// 
    /// Ejemplo:
    /// 
    ///     PUT /api/facility/{id}
    ///     {
    ///         "id": "guid-facility",
    ///         "name": "Planta Actualizada",
    ///         "canDispose": true,
    ///         "maxCapacity": 15000
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FacilityDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<FacilityDto>> Update(Guid id, [FromBody] UpdateFacilityDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var facility = await _facilityService.UpdateAsync(dto, cancellationToken);
            return Ok(facility);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// DELETE: Eliminar facility (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _facilityService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

