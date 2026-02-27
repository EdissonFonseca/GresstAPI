using Gresst.Application.RouteProcesses;
using Gresst.Domain.RouteProcesses;
using HotChocolate.Authorization;

namespace Gresst.API.GraphQL;

/// <summary>
/// GraphQL queries for transport (route process) data.
/// </summary>
[ExtendObjectType("Query")]
[Authorize]
public class TransportQuery
{
    /// <summary>
    /// Get a single route process by id.
    /// </summary>
    public async Task<RouteProcessDto?> GetRouteProcess(
        Guid id,
        [Service] IRouteProcessRepository repository,
        CancellationToken cancellationToken)
    {
        var route = await repository.GetByIdAsync(id, cancellationToken);
        return route == null ? null : RouteProcessMapping.ToDto(route);
    }

    /// <summary>
    /// Get route processes by status (e.g. Planned, InProgress, Completed).
    /// </summary>
    public async Task<IReadOnlyList<RouteProcessDto>> GetRouteProcessesByStatus(
        RouteStatus status,
        [Service] IRouteProcessRepository repository,
        CancellationToken cancellationToken)
    {
        var routes = await repository.GetByStatusAsync(status, cancellationToken);
        return routes.Select(RouteProcessMapping.ToDto).ToList();
    }

    /// <summary>
    /// Get route processes for a given vehicle.
    /// </summary>
    public async Task<IReadOnlyList<RouteProcessDto>> GetRouteProcessesByVehicle(
        Guid vehicleId,
        [Service] IRouteProcessRepository repository,
        CancellationToken cancellationToken)
    {
        var routes = await repository.GetByVehicleIdAsync(vehicleId, cancellationToken);
        return routes.Select(RouteProcessMapping.ToDto).ToList();
    }
}
