using Gresst.Application.Common;
using Gresst.Application.RouteProcesses;
using MediatR;

namespace Gresst.Application.RouteProcesses.Commands.CompleteRouteStop;

/// <summary>
/// Command to complete a route stop and optionally register all waste items
/// handled at that stop. The items are aggregated client-side and sent when
/// the mobile user approves the stop.
/// </summary>
public record CompleteRouteStopCommand(
    Guid RouteProcessId,
    Guid StopId,
    string? Notes,
    IReadOnlyList<string>? WasteItemIds)
    : IRequest<Result<RouteProcessDto>>;
