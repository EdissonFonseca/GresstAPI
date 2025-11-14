using Asp.Versioning;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class RouteController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RouteController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    /// <summary>
    /// GET: Obtener todas las rutas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RouteDto>), 200)]
    public async Task<ActionResult<IEnumerable<RouteDto>>> GetAll(CancellationToken cancellationToken)
    {
        var routes = await _routeService.GetAllAsync(cancellationToken);
        return Ok(routes);
    }

    /// <summary>
    /// GET: Obtener rutas activas
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<RouteDto>), 200)]
    public async Task<ActionResult<IEnumerable<RouteDto>>> GetActive(CancellationToken cancellationToken)
    {
        var routes = await _routeService.GetActiveAsync(cancellationToken);
        return Ok(routes);
    }

    /// <summary>
    /// GET: Obtener rutas por tipo
    /// </summary>
    /// <param name="routeType">Tipo de ruta: Collection, Transport, Delivery</param>
    [HttpGet("type/{routeType}")]
    [ProducesResponseType(typeof(IEnumerable<RouteDto>), 200)]
    public async Task<ActionResult<IEnumerable<RouteDto>>> GetByType(string routeType, CancellationToken cancellationToken)
    {
        var routes = await _routeService.GetByTypeAsync(routeType, cancellationToken);
        return Ok(routes);
    }

    /// <summary>
    /// GET: Obtener rutas por vehículo
    /// </summary>
    [HttpGet("vehicle/{vehicleId}")]
    [ProducesResponseType(typeof(IEnumerable<RouteDto>), 200)]
    public async Task<ActionResult<IEnumerable<RouteDto>>> GetByVehicle(Guid vehicleId, CancellationToken cancellationToken)
    {
        var routes = await _routeService.GetByVehicleAsync(vehicleId, cancellationToken);
        return Ok(routes);
    }

    /// <summary>
    /// GET: Obtener ruta por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RouteDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RouteDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var route = await _routeService.GetByIdAsync(id, cancellationToken);
        
        if (route == null)
            return NotFound(new { message = "Route not found" });

        return Ok(route);
    }

    /// <summary>
    /// POST: Crear nueva ruta
    /// </summary>
    /// <remarks>
    /// El VehicleId es opcional. Si no se proporciona, se usará "SIN-VEHICULO" como placeholder.
    /// 
    /// Ejemplo de request:
    /// 
    ///     POST /api/route
    ///     {
    ///         "code": "RUTA-001",
    ///         "name": "Ruta de Recolección Norte",
    ///         "description": "Ruta de recolección para la zona norte",
    ///         "routeType": "Collection",
    ///         "vehicleId": "guid-vehicle", // Opcional
    ///         "driverId": "guid-driver",
    ///         "schedule": "{\"days\":[\"Monday\",\"Wednesday\",\"Friday\"]}",
    ///         "estimatedDuration": 4.5,
    ///         "estimatedDistance": 120.5,
    ///         "stops": [
    ///             {
    ///                 "sequence": 1,
    ///                 "facilityId": "guid-facility-1",
    ///                 "instructions": "Recoger en la entrada principal"
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(RouteDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<RouteDto>> Create([FromBody] CreateRouteDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var route = await _routeService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = route.Id }, route);
    }

    /// <summary>
    /// PUT: Actualizar ruta existente
    /// </summary>
    /// <remarks>
    /// Solo se actualizan los campos proporcionados (PATCH-like).
    /// Para eliminar el vehículo, enviar vehicleId como null.
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RouteDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RouteDto>> Update(Guid id, [FromBody] UpdateRouteDto dto, CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var route = await _routeService.UpdateAsync(dto, cancellationToken);
        
        if (route == null)
            return NotFound(new { message = "Route not found" });

        return Ok(route);
    }

    /// <summary>
    /// DELETE: Eliminar ruta (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await _routeService.DeactivateAsync(id, cancellationToken);
        
        if (success == null)
            return NotFound(new { message = "Route not found" });

        return NoContent();
    }

    /// <summary>
    /// POST: Agregar parada a una ruta
    /// </summary>
    [HttpPost("{routeId}/stops")]
    [ProducesResponseType(typeof(RouteStopDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RouteStopDto>> AddStop(Guid routeId, [FromBody] CreateRouteStopDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!dto.FacilityId.HasValue)
            return BadRequest(new { message = "FacilityId is required for RouteStop" });

        var stop = await _routeService.AddStopAsync(routeId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = routeId }, stop);
    }

    /// <summary>
    /// DELETE: Eliminar parada de una ruta
    /// </summary>
    [HttpDelete("{routeId}/stops/{facilityId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> RemoveStop(Guid routeId, Guid facilityId, CancellationToken cancellationToken)
    {
        var success = await _routeService.RemoveStopAsync(routeId, facilityId, cancellationToken);
        
        if (!success)
            return NotFound(new { message = "RouteStop not found" });

        return NoContent();
    }

    /// <summary>
    /// PUT: Reordenar paradas de una ruta
    /// </summary>
    /// <remarks>
    /// El diccionario debe contener FacilityId como clave y el nuevo Sequence como valor.
    /// 
    /// Ejemplo:
    /// 
    ///     PUT /api/route/{routeId}/stops/reorder
    ///     {
    ///         "guid-facility-1": 1,
    ///         "guid-facility-2": 2,
    ///         "guid-facility-3": 3
    ///     }
    /// </remarks>
    [HttpPut("{routeId}/stops/reorder")]
    [ProducesResponseType(typeof(RouteDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RouteDto>> ReorderStops(Guid routeId, [FromBody] Dictionary<Guid, int> stopSequences, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var route = await _routeService.ReorderStopsAsync(routeId, stopSequences, cancellationToken);
        return Ok(route);
    }

    /// <summary>
    /// POST: Activar ruta
    /// </summary>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(typeof(RouteDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RouteDto>> Activate(Guid id, CancellationToken cancellationToken)
    {
        var route = await _routeService.ActivateAsync(id, cancellationToken);
        
        if (route == null)
            return NotFound(new { message = "Route not found" });

        return Ok(route);
    }

    /// <summary>
    /// POST: Desactivar ruta
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [ProducesResponseType(typeof(RouteDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RouteDto>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var route = await _routeService.DeactivateAsync(id, cancellationToken);
        
        if (route == null)
            return NotFound(new { message = "Route not found" });

        return Ok(route);
    }
}

