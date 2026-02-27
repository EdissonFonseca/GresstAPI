using Gresst.Domain.Common;
using Gresst.Domain.Entities;
using Gresst.Domain.RouteProcesses;
using Gresst.Domain.RouteProcesses.Events;
using MediatR;

namespace Gresst.Application.RouteProcesses.EventHandlers;

/// <summary>
/// Handles ResidueRelocationTriggeredEvent.
/// Creates an OperationType.Relocation record to update residue location.
/// </summary>
public class ResidueRelocationTriggeredHandler
    : INotificationHandler<ResidueRelocationTriggeredEvent>
{
    private readonly IRouteProcessRepository _routeRepository;
    private readonly IWasteOperationRepository _operationRepository;

    public ResidueRelocationTriggeredHandler(
        IRouteProcessRepository routeRepository,
        IWasteOperationRepository operationRepository)
    {
        _routeRepository = routeRepository;
        _operationRepository = operationRepository;
    }

    public async Task Handle(ResidueRelocationTriggeredEvent notification, CancellationToken ct)
    {
        var route = await _routeRepository.GetByIdAsync(notification.RouteProcessId, ct);
        if (route == null) return;

        var stop = route.Stops.FirstOrDefault(s => s.Id == notification.StopId);
        if (stop == null) return;

        // Pickup: from = stop location (generator), to = vehicle. Delivery: from = vehicle, to = stop location.
        string fromLocationId;
        string toLocationId;
        if (stop.OperationType == StopOperationType.Pickup)
        {
            fromLocationId = notification.LocationId;
            toLocationId = $"vehicle:{notification.VehicleId}";
        }
        else
        {
            fromLocationId = $"vehicle:{notification.VehicleId}";
            toLocationId = notification.LocationId;
        }

        var data = new RelocationData(fromLocationId, toLocationId, notification.VehicleId);
        var operation = new WasteOperation(
            OperationType.Relocation,
            data,
            notification.RouteProcessId,
            notification.OccurredOn);

        await _operationRepository.AddAsync(operation, ct);
    }
}

/// <summary>
/// Handles ResidueTransferTriggeredEvent.
/// Creates an OperationType.Transfer record to update residue ownership.
/// </summary>
public class ResidueTransferTriggeredHandler
    : INotificationHandler<ResidueTransferTriggeredEvent>
{
    private readonly IWasteOperationRepository _operationRepository;

    public ResidueTransferTriggeredHandler(IWasteOperationRepository operationRepository)
        => _operationRepository = operationRepository;

    public async Task Handle(ResidueTransferTriggeredEvent notification, CancellationToken ct)
    {
        var data = new TransferData(notification.FromPartyId, notification.ToPartyId);
        var operation = new WasteOperation(
            OperationType.Transfer,
            data,
            notification.RouteProcessId,
            notification.OccurredOn);

        await _operationRepository.AddAsync(operation, ct);
    }
}

/// <summary>
/// Handles ResidueStorageTriggeredEvent.
/// Creates an OperationType.Storage record.
/// </summary>
public class ResidueStorageTriggeredHandler
    : INotificationHandler<ResidueStorageTriggeredEvent>
{
    private readonly IWasteOperationRepository _operationRepository;

    public ResidueStorageTriggeredHandler(IWasteOperationRepository operationRepository)
        => _operationRepository = operationRepository;

    public async Task Handle(ResidueStorageTriggeredEvent notification, CancellationToken ct)
    {
        var data = new StorageData(notification.LocationId);
        var operation = new WasteOperation(
            OperationType.Storage,
            data,
            notification.RouteProcessId,
            notification.OccurredOn);

        await _operationRepository.AddAsync(operation, ct);
    }
}
