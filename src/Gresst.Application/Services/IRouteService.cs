using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IRouteService
{
    Task<RouteDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetByTypeAsync(string routeType, CancellationToken cancellationToken = default);
    Task<IEnumerable<RouteDto>> GetByVehicleAsync(string vehicleId, CancellationToken cancellationToken = default);
    Task<RouteDto> CreateAsync(CreateRouteDto dto, CancellationToken cancellationToken = default);
    Task<RouteDto?> UpdateAsync(UpdateRouteDto dto, CancellationToken cancellationToken = default);
    Task<RouteStopDto> AddStopAsync(string routeId, CreateRouteStopDto dto, CancellationToken cancellationToken = default);
    Task<bool> RemoveStopAsync(string routeId, string facilityId, CancellationToken cancellationToken = default);
    Task<RouteDto> ReorderStopsAsync(string routeId, Dictionary<string, int> stopSequences, CancellationToken cancellationToken = default);
    Task<RouteDto?> ActivateAsync(string id, CancellationToken cancellationToken = default);
    Task<RouteDto?> DeactivateAsync(string id, CancellationToken cancellationToken = default);
}

