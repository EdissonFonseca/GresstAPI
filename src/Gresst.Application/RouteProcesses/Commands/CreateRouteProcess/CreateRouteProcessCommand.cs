using Gresst.Application.Common;
using Gresst.Domain.RouteProcesses;
using MediatR;

namespace Gresst.Application.RouteProcesses.Commands.CreateRouteProcess;

public record CreateRouteStopInput(string LocationId, StopOperationType OperationType, Guid? ResponsiblePartyId);

public record CreateRouteProcessCommand(
    Guid VehicleId,
    Guid DriverId,
    IReadOnlyList<CreateRouteStopInput> Stops
) : IRequest<Result<RouteProcessDto>>;
