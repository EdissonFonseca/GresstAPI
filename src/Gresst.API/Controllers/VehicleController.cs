using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>
    /// GET: Obtener todos los vehículos - Por defecto para la persona de la cuenta (con segmentación)
    /// </summary>
    /// <remarks>
    /// Devuelve todos los vehículos activos a los que el usuario actual tiene acceso (segmentación).
    /// Por defecto muestra los vehículos de la persona de la cuenta.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), 200)]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleService.GetAccountPersonVehiclesAsync(cancellationToken);
        return Ok(vehicles);
    }

    /// <summary>
    /// GET: Obtener vehículos de la persona de la cuenta (explícito)
    /// </summary>
    [HttpGet("account")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), 200)]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAccountPersonVehicles(CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleService.GetAccountPersonVehiclesAsync(cancellationToken);
        return Ok(vehicles);
    }

    /// <summary>
    /// GET: Obtener vehículos de un proveedor
    /// </summary>
    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), 200)]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetProviderVehicles(Guid providerId, CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleService.GetProviderVehiclesAsync(providerId, cancellationToken);
        return Ok(vehicles);
    }

    /// <summary>
    /// GET: Obtener vehículos de un cliente
    /// </summary>
    [HttpGet("client/{clientId}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), 200)]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetClientVehicles(Guid clientId, CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleService.GetClientVehiclesAsync(clientId, cancellationToken);
        return Ok(vehicles);
    }

    /// <summary>
    /// GET: Obtener vehículo por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VehicleDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<VehicleDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id, cancellationToken);
        
        if (vehicle == null)
            return NotFound(new { message = "Vehicle not found or you don't have access" });

        return Ok(vehicle);
    }

    /// <summary>
    /// POST: Crear nuevo vehículo - Por defecto para la persona de la cuenta
    /// </summary>
    /// <remarks>
    /// Si no se especifica PersonId, se crea para la persona de la cuenta.
    /// 
    /// Ejemplo de request:
    /// 
    ///     POST /api/vehicle
    ///     {
    ///         "licensePlate": "ABC123",
    ///         "vehicleType": "Truck",
    ///         "model": "Freightliner",
    ///         "make": "Mercedes-Benz",
    ///         "year": 2020,
    ///         "maxCapacity": 5000,
    ///         "capacityUnit": "kg"
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<VehicleDto>> Create([FromBody] CreateVehicleDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var vehicle = await _vehicleService.CreateAccountPersonVehicleAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    /// <summary>
    /// POST: Crear vehículo para la persona de la cuenta (explícito)
    /// </summary>
    [HttpPost("account")]
    [ProducesResponseType(typeof(VehicleDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<VehicleDto>> CreateAccountPersonVehicle([FromBody] CreateVehicleDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var vehicle = await _vehicleService.CreateAccountPersonVehicleAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    /// <summary>
    /// POST: Crear vehículo para un proveedor
    /// </summary>
    [HttpPost("provider/{providerId}")]
    [ProducesResponseType(typeof(VehicleDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<VehicleDto>> CreateProviderVehicle(Guid providerId, [FromBody] CreateVehicleDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var vehicle = await _vehicleService.CreateProviderVehicleAsync(providerId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    /// <summary>
    /// POST: Crear vehículo para un cliente
    /// </summary>
    [HttpPost("client/{clientId}")]
    [ProducesResponseType(typeof(VehicleDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<VehicleDto>> CreateClientVehicle(Guid clientId, [FromBody] CreateVehicleDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var vehicle = await _vehicleService.CreateClientVehicleAsync(clientId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, vehicle);
    }

    /// <summary>
    /// PUT: Actualizar vehículo existente
    /// </summary>
    /// <remarks>
    /// Solo se actualizan los campos proporcionados (PATCH-like).
    /// 
    /// Ejemplo:
    /// 
    ///     PUT /api/vehicle/{id}
    ///     {
    ///         "id": "guid-vehiculo",
    ///         "licensePlate": "ABC123",
    ///         "isAvailable": false
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(VehicleDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<VehicleDto>> Update(Guid id, [FromBody] UpdateVehicleDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var vehicle = await _vehicleService.UpdateAsync(dto, cancellationToken);
        
        if (vehicle == null)
            return NotFound(new { message = "Vehicle not found or you don't have access" });

        return Ok(vehicle);
    }

    /// <summary>
    /// DELETE: Eliminar vehículo (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _vehicleService.DeleteAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "Vehicle not found or you don't have access" });

        return NoContent();
    }
}

