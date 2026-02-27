using Gresst.Application.RouteProcesses.Commands.CancelRouteProcess;
using Gresst.Application.RouteProcesses.Commands.CompleteRouteStop;
using Gresst.Application.RouteProcesses.Commands.CreateRouteProcess;
using Gresst.Application.RouteProcesses.Commands.StartRouteProcess;
using Gresst.Domain.RouteProcesses;
using Gresst.GraphQL.RouteProcesses.Inputs;
using Gresst.GraphQL.RouteProcesses.Payloads;
using MediatR;

namespace Gresst.GraphQL.RouteProcesses;

[MutationType]
public class RouteProcessMutations
{
    /// <summary>
    /// Creates a new planned transport route with its stops.
    /// </summary>
    /// <example>
    /// mutation {
    ///   createRouteProcess(input: {
    ///     vehicleId: "...",
    ///     driverId: "...",
    ///     stops: [
    ///       { locationId: "loc-1", operationType: PICKUP, responsiblePartyId: null },
    ///       { locationId: "loc-2", operationType: DELIVERY, responsiblePartyId: "party-abc" }
    ///     ]
    ///   }) {
    ///     isSuccess
    ///     routeProcess { id status stops { order operationType isCompleted } }
    ///     error
    ///   }
    /// }
    /// </example>
    public async Task<RouteProcessPayload> CreateRouteProcessAsync(
        CreateRouteProcessInput input,
        [Service] ISender sender,
        CancellationToken ct)
    {
        var command = new CreateRouteProcessCommand(
            input.VehicleId,
            input.DriverId,
            input.Stops
                .Select(s => new CreateRouteStopInput(
                    s.LocationId,
                    MapStopOperationType(s.OperationType),
                    s.ResponsiblePartyId))
                .ToList()
        );

        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? RouteProcessPayload.Success(result.Value!)
            : RouteProcessPayload.Fail(result.Error!);
    }

    /// <summary>
    /// Starts a planned route — assigns it to the vehicle and driver.
    /// </summary>
    public async Task<RouteProcessPayload> StartRouteProcessAsync(
        StartRouteProcessInput input,
        [Service] ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new StartRouteProcessCommand(input.RouteProcessId), ct);
        return result.IsSuccess
            ? RouteProcessPayload.Success(result.Value!)
            : RouteProcessPayload.Fail(result.Error!);
    }

    /// <summary>
    /// Marks a stop as completed.
    /// Automatically triggers the appropriate Operations:
    ///   - PICKUP     → Relocation
    ///   - DELIVERY   → Relocation + Transfer
    ///   - INTERMEDIATE_STORAGE → Storage
    ///   - CUSTODY_TRANSFER     → Transfer
    /// If all stops are completed, the route is automatically completed.
    /// </summary>
    public async Task<RouteProcessPayload> CompleteRouteStopAsync(
        CompleteRouteStopInput input,
        [Service] ISender sender,
        CancellationToken ct)
    {
        var command = new CompleteRouteStopCommand(
            input.RouteProcessId,
            input.StopId,
            input.Notes);

        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? RouteProcessPayload.Success(result.Value!)
            : RouteProcessPayload.Fail(result.Error!);
    }

    /// <summary>Cancels an in-progress or planned route.</summary>
    public async Task<RouteProcessPayload> CancelRouteProcessAsync(
        CancelRouteProcessInput input,
        [Service] ISender sender,
        CancellationToken ct)
    {
        var command = new CancelRouteProcessCommand(input.RouteProcessId, input.Reason);
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? RouteProcessPayload.Success(result.Value!)
            : RouteProcessPayload.Fail(result.Error!);
    }

    private static StopOperationType MapStopOperationType(StopOperationTypeInput input) => input switch
    {
        StopOperationTypeInput.Pickup => StopOperationType.Pickup,
        StopOperationTypeInput.Delivery => StopOperationType.Delivery,
        StopOperationTypeInput.IntermediateStorage => StopOperationType.IntermediateStorage,
        StopOperationTypeInput.CustodyTransfer => StopOperationType.CustodyTransfer,
        _ => StopOperationType.Pickup
    };
}
