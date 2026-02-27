using Gresst.Application.Common;
using Gresst.Application.RouteProcesses;
using MediatR;

namespace Gresst.Application.RouteProcesses.Commands.CompleteRouteStop;

public record CompleteRouteStopCommand(Guid RouteProcessId, Guid StopId, string? Notes)
    : IRequest<Result<RouteProcessDto>>;
