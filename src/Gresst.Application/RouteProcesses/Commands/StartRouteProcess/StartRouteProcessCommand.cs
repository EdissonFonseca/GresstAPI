using Gresst.Application.Common;
using Gresst.Application.RouteProcesses;
using MediatR;

namespace Gresst.Application.RouteProcesses.Commands.StartRouteProcess;

public record StartRouteProcessCommand(Guid RouteProcessId) : IRequest<Result<RouteProcessDto>>;
