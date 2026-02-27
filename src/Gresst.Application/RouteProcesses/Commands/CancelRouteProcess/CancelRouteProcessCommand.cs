using Gresst.Application.Common;
using Gresst.Application.RouteProcesses;
using MediatR;

namespace Gresst.Application.RouteProcesses.Commands.CancelRouteProcess;

public record CancelRouteProcessCommand(Guid RouteProcessId, string Reason) : IRequest<Result<RouteProcessDto>>;
