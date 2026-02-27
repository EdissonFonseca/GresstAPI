using Gresst.Application.RouteProcesses;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace Gresst.GraphQL.RouteProcesses;

/// <summary>
/// Real-time subscriptions for route tracking.
/// Useful for driver apps or dispatch dashboards.
/// </summary>
[SubscriptionType]
public class RouteProcessSubscriptions
{
    /// <summary>
    /// Subscribe to all updates of a specific route process.
    /// Fires on: stop completed, route started, route completed, route cancelled.
    /// </summary>
    /// <example>
    /// subscription {
    ///   onRouteProcessUpdated(routeProcessId: "...") {
    ///     id
    ///     status
    ///     progressPercent
    ///     stops { order isCompleted completedAt }
    ///   }
    /// }
    /// </example>
    [Subscribe]
    [Topic($"{{{nameof(routeProcessId)}}}")]
    public RouteProcessDto OnRouteProcessUpdated(
        Guid routeProcessId,
        [EventMessage] RouteProcessDto updatedRouteProcess) => updatedRouteProcess;
}

/// <summary>
/// Service to publish route updates to subscribers.
/// Inject this into your domain event handlers.
/// </summary>
public class RouteProcessEventPublisher
{
    private readonly ITopicEventSender _sender;

    public RouteProcessEventPublisher(ITopicEventSender sender)
        => _sender = sender;

    public async Task PublishRouteUpdatedAsync(RouteProcessDto dto, CancellationToken ct = default) =>
        await _sender.SendAsync(dto.Id.ToString(), dto, ct);
}
