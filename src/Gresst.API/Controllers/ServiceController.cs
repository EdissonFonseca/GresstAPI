using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ServiceController : ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    // Service CRUD endpoints
    /// <summary>
    /// GET: Obtener todos los servicios disponibles
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(IEnumerable<ServiceDto>), 200)]
    public async Task<ActionResult<IEnumerable<ServiceDto>>> GetAllServices(CancellationToken cancellationToken)
    {
        var services = await _serviceService.GetAllServicesAsync(cancellationToken);
        return Ok(services);
    }

    /// <summary>
    /// GET: Obtener servicio por ID
    /// </summary>
    [HttpGet("types/{id}")]
    [ProducesResponseType(typeof(ServiceDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ServiceDto>> GetServiceById(Guid id, CancellationToken cancellationToken)
    {
        var service = await _serviceService.GetServiceByIdAsync(id, cancellationToken);
        
        if (service == null)
            return NotFound(new { message = "Service not found" });

        return Ok(service);
    }

    /// <summary>
    /// POST: Crear nuevo tipo de servicio
    /// </summary>
    [HttpPost("types")]
    [ProducesResponseType(typeof(ServiceDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ServiceDto>> CreateService([FromBody] CreateServiceDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var service = await _serviceService.CreateServiceAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, service);
    }

    /// <summary>
    /// PUT: Actualizar tipo de servicio
    /// </summary>
    [HttpPut("types/{id}")]
    [ProducesResponseType(typeof(ServiceDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ServiceDto>> UpdateService(Guid id, [FromBody] UpdateServiceDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var service = await _serviceService.UpdateServiceAsync(dto, cancellationToken);
        
        if (service == null)
            return NotFound(new { message = "Service not found" });

        return Ok(service);
    }

    /// <summary>
    /// DELETE: Eliminar tipo de servicio (soft delete)
    /// </summary>
    [HttpDelete("types/{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteService(Guid id, CancellationToken cancellationToken)
    {
        var success = await _serviceService.DeleteServiceAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "Service not found" });

        return NoContent();
    }

    // PersonService - Account Person endpoints
    /// <summary>
    /// GET: Obtener servicios de la persona de la cuenta
    /// </summary>
    [HttpGet("account")]
    [ProducesResponseType(typeof(IEnumerable<PersonServiceDto>), 200)]
    public async Task<ActionResult<IEnumerable<PersonServiceDto>>> GetAccountPersonServices(CancellationToken cancellationToken)
    {
        var services = await _serviceService.GetAccountPersonServicesAsync(cancellationToken);
        return Ok(services);
    }

    /// <summary>
    /// POST: Crear servicio para la persona de la cuenta
    /// </summary>
    /// <remarks>
    /// Ejemplo de request:
    /// 
    ///     POST /api/service/account
    ///     {
    ///         "serviceId": "guid-service",
    ///         "startDate": "2024-01-01T00:00:00Z",
    ///         "endDate": null
    ///     }
    /// </remarks>
    [HttpPost("account")]
    [ProducesResponseType(typeof(PersonServiceDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PersonServiceDto>> CreateAccountPersonService([FromBody] CreatePersonServiceDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personService = await _serviceService.CreateAccountPersonServiceAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetAccountPersonServices), new { }, personService);
    }

    /// <summary>
    /// PUT: Actualizar servicio de la persona de la cuenta
    /// </summary>
    [HttpPut("account")]
    [ProducesResponseType(typeof(PersonServiceDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PersonServiceDto>> UpdateAccountPersonService([FromBody] UpdatePersonServiceDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var personService = await _serviceService.UpdateAccountPersonServiceAsync(dto, cancellationToken);
        
        if (personService == null)
            return NotFound(new { message = "PersonService not found" });

        return Ok(personService);
    }

    /// <summary>
    /// DELETE: Eliminar servicio de la persona de la cuenta
    /// </summary>
    [HttpDelete("account/{serviceId}/{startDate}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteAccountPersonService(Guid serviceId, DateTime startDate, CancellationToken cancellationToken)
    {
        var success = await _serviceService.DeleteAccountPersonServiceAsync(serviceId, startDate, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "PersonService not found" });

        return NoContent();
    }

    // Note: Provider service endpoints have been moved to PersonController
    // Use GET /api/person/{personId}/service, POST /api/person/{personId}/service, etc. instead
}

