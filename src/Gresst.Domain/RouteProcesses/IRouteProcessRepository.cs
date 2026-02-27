namespace Gresst.Domain.RouteProcesses;

/// <summary>
/// Repository for the RouteProcess aggregate (Transport process).
/// </summary>
public interface IRouteProcessRepository
{
    Task<RouteProcess?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<RouteProcess>> GetByStatusAsync(RouteStatus status, CancellationToken ct = default);
    Task<IReadOnlyList<RouteProcess>> GetByVehicleIdAsync(Guid vehicleId, CancellationToken ct = default);
    Task AddAsync(RouteProcess routeProcess, CancellationToken ct = default);
    Task UpdateAsync(RouteProcess routeProcess, CancellationToken ct = default);
}
