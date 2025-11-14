using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class WasteClassController : ControllerBase
{
    private readonly IWasteClassService _wasteClassService;

    public WasteClassController(IWasteClassService wasteClassService)
    {
        _wasteClassService = wasteClassService;
    }

    // WasteClass CRUD endpoints
    /// <summary>
    /// GET: Obtener todas las clases de residuo disponibles
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<WasteClassDto>), 200)]
    public async Task<ActionResult<IEnumerable<WasteClassDto>>> GetAllWasteClasses(CancellationToken cancellationToken)
    {
        var wasteClasses = await _wasteClassService.GetAllWasteClassesAsync(cancellationToken);
        return Ok(wasteClasses);
    }

    /// <summary>
    /// GET: Obtener clase de residuo por ID
    /// </summary>
    [HttpGet("types/{id}")]
    [ProducesResponseType(typeof(WasteClassDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<WasteClassDto>> GetWasteClassById(Guid id, CancellationToken cancellationToken)
    {
        var wasteClass = await _wasteClassService.GetWasteClassByIdAsync(id, cancellationToken);
        
        if (wasteClass == null)
            return NotFound(new { message = "WasteClass not found" });

        return Ok(wasteClass);
    }

    /// <summary>
    /// POST: Crear nueva clase de residuo
    /// </summary>
    [HttpPost("types")]
    [ProducesResponseType(typeof(WasteClassDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<WasteClassDto>> CreateWasteClass([FromBody] CreateWasteClassDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var wasteClass = await _wasteClassService.CreateWasteClassAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetWasteClassById), new { id = wasteClass.Id }, wasteClass);
    }

    /// <summary>
    /// PUT: Actualizar clase de residuo
    /// </summary>
    [HttpPut("types/{id}")]
    [ProducesResponseType(typeof(WasteClassDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<WasteClassDto>> UpdateWasteClass(Guid id, [FromBody] UpdateWasteClassDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var wasteClass = await _wasteClassService.UpdateWasteClassAsync(dto, cancellationToken);
        
        if (wasteClass == null)
            return NotFound(new { message = "WasteClass not found" });

        return Ok(wasteClass);
    }

    /// <summary>
    /// DELETE: Eliminar clase de residuo (soft delete)
    /// </summary>
    [HttpDelete("types/{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteWasteClass(Guid id, CancellationToken cancellationToken)
    {
        var success = await _wasteClassService.DeleteWasteClassAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "WasteClass not found" });

        return NoContent();
    }

    // PersonWasteClass - Account Person endpoints
    /// <summary>
    /// GET: Obtener clases de residuo de la persona de la cuenta
    /// </summary>
    [HttpGet("account")]
    [ProducesResponseType(typeof(IEnumerable<PersonWasteClassDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonWasteClassDto>>> GetAccountPersonWasteClasses(CancellationToken cancellationToken)
    {
        var wasteClasses = await _wasteClassService.GetAccountPersonWasteClassesAsync(cancellationToken);
        return Ok(wasteClasses);
    }

    /// <summary>
    /// POST: Crear asociación de clase de residuo para la persona de la cuenta
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/wasteclass/account
    ///     {
    ///         "wasteClassId": "guid-waste-class"
    ///     }
    /// </remarks>
    [HttpPost("account")]
    [ProducesResponseType(typeof(PersonWasteClassDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonWasteClassDto>> CreateAccountPersonWasteClass([FromBody] CreatePersonWasteClassDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personWasteClass = await _wasteClassService.CreateAccountPersonWasteClassAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetAccountPersonWasteClasses), new { }, personWasteClass);
    }

    /// <summary>
    /// PUT: Actualizar asociación de clase de residuo de la persona de la cuenta
    /// </summary>
    [HttpPut("account")]
    [ProducesResponseType(typeof(PersonWasteClassDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonWasteClassDto>> UpdateAccountPersonWasteClass([FromBody] UpdatePersonWasteClassDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personWasteClass = await _wasteClassService.UpdateAccountPersonWasteClassAsync(dto, cancellationToken);
        
        if (personWasteClass == null)
            return NotFound(new { message = "PersonWasteClass not found" });

        return Ok(personWasteClass);
    }

    /// <summary>
    /// DELETE: Eliminar asociación de clase de residuo de la persona de la cuenta
    /// </summary>
    [HttpDelete("account/{wasteClassId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteAccountPersonWasteClass(Guid wasteClassId, CancellationToken cancellationToken)
    {
        var success = await _wasteClassService.DeleteAccountPersonWasteClassAsync(wasteClassId, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "PersonWasteClass not found" });

        return NoContent();
    }

    // PersonWasteClass - Provider endpoints
    /// <summary>
    /// GET: Obtener clases de residuo de un proveedor
    /// </summary>
    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(typeof(IEnumerable<PersonWasteClassDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonWasteClassDto>>> GetProviderWasteClasses(Guid providerId, CancellationToken cancellationToken)
    {
        var wasteClasses = await _wasteClassService.GetProviderWasteClassesAsync(providerId, cancellationToken);
        return Ok(wasteClasses);
    }

    /// <summary>
    /// POST: Crear asociación de clase de residuo para un proveedor
    /// </summary>
    [HttpPost("provider/{providerId}")]
    [ProducesResponseType(typeof(PersonWasteClassDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonWasteClassDto>> CreateProviderWasteClass(Guid providerId, [FromBody] CreatePersonWasteClassDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personWasteClass = await _wasteClassService.CreateProviderWasteClassAsync(providerId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetProviderWasteClasses), new { providerId }, personWasteClass);
    }

    /// <summary>
    /// PUT: Actualizar asociación de clase de residuo de un proveedor
    /// </summary>
    [HttpPut("provider/{providerId}")]
    [ProducesResponseType(typeof(PersonWasteClassDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonWasteClassDto>> UpdateProviderWasteClass(Guid providerId, [FromBody] UpdatePersonWasteClassDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personWasteClass = await _wasteClassService.UpdateProviderWasteClassAsync(providerId, dto, cancellationToken);
        
        if (personWasteClass == null)
            return NotFound(new { message = "PersonWasteClass not found" });

        return Ok(personWasteClass);
    }

    /// <summary>
    /// DELETE: Eliminar asociación de clase de residuo de un proveedor
    /// </summary>
    [HttpDelete("provider/{providerId}/{wasteClassId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteProviderWasteClass(Guid providerId, Guid wasteClassId, CancellationToken cancellationToken)
    {
        var success = await _wasteClassService.DeleteProviderWasteClassAsync(providerId, wasteClassId, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "PersonWasteClass not found" });

        return NoContent();
    }
}

