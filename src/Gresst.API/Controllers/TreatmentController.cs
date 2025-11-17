using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TreatmentController : ControllerBase
{
    private readonly ITreatmentService _treatmentService;

    public TreatmentController(ITreatmentService treatmentService)
    {
        _treatmentService = treatmentService;
    }

    // Treatment CRUD endpoints
    /// <summary>
    /// GET: Obtener todos los tratamientos disponibles
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<TreatmentDto>), 200)]
    public async Task<ActionResult<IEnumerable<TreatmentDto>>> GetAllTreatments(CancellationToken cancellationToken)
    {
        var treatments = await _treatmentService.GetAllTreatmentsAsync(cancellationToken);
        return Ok(treatments);
    }

    /// <summary>
    /// GET: Obtener tratamiento por ID
    /// </summary>
    [HttpGet("types/{id}")]
    [ProducesResponseType(typeof(TreatmentDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TreatmentDto>> GetTreatmentById(Guid id, CancellationToken cancellationToken)
    {
        var treatment = await _treatmentService.GetTreatmentByIdAsync(id, cancellationToken);
        
        if (treatment == null)
            return NotFound(new { message = "Treatment not found" });

        return Ok(treatment);
    }

    /// <summary>
    /// POST: Crear nuevo tratamiento
    /// </summary>
    [HttpPost("types")]
    [ProducesResponseType(typeof(TreatmentDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<TreatmentDto>> CreateTreatment([FromBody] CreateTreatmentDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var treatment = await _treatmentService.CreateTreatmentAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetTreatmentById), new { id = treatment.Id }, treatment);
    }

    /// <summary>
    /// PUT: Actualizar tratamiento
    /// </summary>
    [HttpPut("types/{id}")]
    [ProducesResponseType(typeof(TreatmentDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<TreatmentDto>> UpdateTreatment(Guid id, [FromBody] UpdateTreatmentDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var treatment = await _treatmentService.UpdateTreatmentAsync(dto, cancellationToken);
        
        if (treatment == null)
            return NotFound(new { message = "Treatment not found" });

        return Ok(treatment);
    }

    /// <summary>
    /// DELETE: Eliminar tratamiento (soft delete)
    /// </summary>
    [HttpDelete("types/{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteTreatment(Guid id, CancellationToken cancellationToken)
    {
        var success = await _treatmentService.DeleteTreatmentAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "Treatment not found" });

        return NoContent();
    }

    // PersonTreatment - Account Person endpoints
    /// <summary>
    /// GET: Obtener tratamientos de la persona de la cuenta
    /// </summary>
    [HttpGet("account")]
    [ProducesResponseType(typeof(IEnumerable<PersonTreatmentDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonTreatmentDto>>> GetAccountPersonTreatments(CancellationToken cancellationToken)
    {
        var treatments = await _treatmentService.GetAccountPersonTreatmentsAsync(cancellationToken);
        return Ok(treatments);
    }

    /// <summary>
    /// POST: Crear asociación de tratamiento para la persona de la cuenta
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/treatment/account
    ///     {
    ///         "treatmentId": "guid-treatment",
    ///         "isManaged": true,
    ///         "canTransfer": false
    ///     }
    /// </remarks>
    [HttpPost("account")]
    [ProducesResponseType(typeof(PersonTreatmentDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonTreatmentDto>> CreateAccountPersonTreatment([FromBody] CreatePersonTreatmentDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personTreatment = await _treatmentService.CreateAccountPersonTreatmentAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetAccountPersonTreatments), new { }, personTreatment);
    }

    /// <summary>
    /// PUT: Actualizar asociación de tratamiento de la persona de la cuenta
    /// </summary>
    [HttpPut("account")]
    [ProducesResponseType(typeof(PersonTreatmentDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonTreatmentDto>> UpdateAccountPersonTreatment([FromBody] UpdatePersonTreatmentDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personTreatment = await _treatmentService.UpdateAccountPersonTreatmentAsync(dto, cancellationToken);
        
        if (personTreatment == null)
            return NotFound(new { message = "PersonTreatment not found" });

        return Ok(personTreatment);
    }

    /// <summary>
    /// DELETE: Eliminar asociación de tratamiento de la persona de la cuenta
    /// </summary>
    [HttpDelete("account/{treatmentId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteAccountPersonTreatment(Guid treatmentId, CancellationToken cancellationToken)
    {
        var success = await _treatmentService.DeleteAccountPersonTreatmentAsync(treatmentId, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "PersonTreatment not found" });

        return NoContent();
    }

    // Note: Provider treatment endpoints have been moved to PersonController
    // Use GET /api/person/{personId}/treatment, POST /api/person/{personId}/treatment, etc. instead
}

