using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IRouteService
{
    Task<RouteDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetByTypeAsync(string routeType, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<RouteDto> CreateAsync(CreateRouteDto dto, CancellationToken cancellationToken = default);
    Task<RouteDto> UpdateAsync(dynamic dto, CancellationToken cancellationToken = default);
    Task<RouteDto> AddStopAsync(Guid routeId, dynamic dto, CancellationToken cancellationToken = default);
    Task RemoveStopAsync(Guid routeId, Guid stopId, CancellationToken cancellationToken = default);
    Task<RouteDto> ReorderStopsAsync(Guid routeId, Dictionary<Guid, int> stopSequences, CancellationToken cancellationToken = default);
    Task<RouteDto> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RouteDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RouteDto> OptimizeRouteAsync(Guid id, CancellationToken cancellationToken = default);
}

