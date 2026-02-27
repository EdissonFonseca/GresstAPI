using Gresst.Domain.Common;
using Gresst.Domain.RouteProcesses.Events;

namespace Gresst.Domain.RouteProcesses;

/// <summary>
/// Aggregate Root for the Transport process.
/// 
/// Orchestrates the following OperationTypes:
///   - Relocation   → triggered on Pickup and Delivery stops
///   - Transfer     → triggered on Delivery and CustodyTransfer stops
///   - Storage      → triggered on IntermediateStorage stops
/// 
/// All state changes go through this aggregate. No direct mutations from outside.
/// </summary>
public class RouteProcess : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid VehicleId { get; private set; }
    public Guid DriverId { get; private set; }
    public RouteStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    private readonly List<RouteStop> _stops = new();
    public IReadOnlyList<RouteStop> Stops => _stops.AsReadOnly();

    // For EF Core
    private RouteProcess() { }

    // -------------------------------------------------------------------------
    // Factory
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a new planned RouteProcess.
    /// Stops must be provided in the desired order (Order property is reassigned).
    /// </summary>
    public static RouteProcess Create(
        Guid vehicleId,
        Guid driverId,
        IEnumerable<(string LocationId, StopOperationType OperationType, Guid? ResponsiblePartyId)> stops)
    {
        var stopList = stops.ToList();

        if (stopList.Count == 0)
            throw new RouteProcessDomainException("A route must have at least one stop.");

        var hasPickup = stopList.Any(s => s.OperationType == StopOperationType.Pickup);
        if (!hasPickup)
            throw new RouteProcessDomainException("Transport route must have at least one Pickup stop.");

        var route = new RouteProcess
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            DriverId = driverId,
            Status = RouteStatus.Planned,
            CreatedAt = DateTime.UtcNow
        };

        for (int i = 0; i < stopList.Count; i++)
        {
            var (locationId, opType, partyId) = stopList[i];
            route._stops.Add(new RouteStop(route.Id, locationId, i + 1, opType, partyId));
        }

        route.AddDomainEvent(new RouteProcessCreatedEvent(route.Id, vehicleId, driverId));

        return route;
    }

    // -------------------------------------------------------------------------
    // Business operations
    // -------------------------------------------------------------------------

    public void Start()
    {
        EnsureStatus(RouteStatus.Planned, "Only planned routes can be started.");

        Status = RouteStatus.InProgress;
        StartedAt = DateTime.UtcNow;

        AddDomainEvent(new RouteProcessStartedEvent(Id, VehicleId, DriverId, StartedAt.Value));
    }

    /// <summary>
    /// Completes a stop and dispatches the corresponding domain events
    /// based on the operation type at that stop.
    /// </summary>
    public void CompleteStop(
        Guid stopId,
        string? notes = null,
        IReadOnlyList<string>? wasteItemIds = null)
    {
        EnsureStatus(RouteStatus.InProgress, "Route must be in progress to complete a stop.");

        var stop = _stops.FirstOrDefault(s => s.Id == stopId)
            ?? throw new RouteProcessDomainException($"Stop {stopId} not found in route {Id}.");

        if (stop.IsCompleted)
            throw new RouteProcessDomainException($"Stop {stopId} is already completed.");

        stop.Complete(notes);

        var itemIds = (wasteItemIds ?? Array.Empty<string>()).ToArray();

        // Dispatch operation-specific domain events.
        // Downstream handlers will create the actual Operations (Relocation, Transfer, etc.)
        switch (stop.OperationType)
        {
            case StopOperationType.Pickup:
                // Residues are loaded onto the vehicle → Relocation (origin → vehicle)
                AddDomainEvent(new ResidueRelocationTriggeredEvent(
                    RouteProcessId: Id,
                    StopId: stop.Id,
                    LocationId: stop.LocationId,
                    VehicleId: VehicleId,
                    WasteItemIds: itemIds,
                    OccurredOn: stop.CompletedAt!.Value));
                break;

            case StopOperationType.Delivery:
                // Residues arrive at destination → Relocation (vehicle → destination) + Transfer of ownership
                AddDomainEvent(new ResidueRelocationTriggeredEvent(
                    RouteProcessId: Id,
                    StopId: stop.Id,
                    LocationId: stop.LocationId,
                    VehicleId: VehicleId,
                    WasteItemIds: itemIds,
                    OccurredOn: stop.CompletedAt!.Value));
                AddDomainEvent(new ResidueTransferTriggeredEvent(
                    RouteProcessId: Id,
                    StopId: stop.Id,
                    FromPartyId: DriverId,
                    ToPartyId: stop.ResponsiblePartyId
                        ?? throw new RouteProcessDomainException("Delivery stop requires a ResponsiblePartyId (receiver)."),
                    WasteItemIds: itemIds,
                    OccurredOn: stop.CompletedAt!.Value));
                break;

            case StopOperationType.IntermediateStorage:
                // Residues temporarily stored → Storage operation
                AddDomainEvent(new ResidueStorageTriggeredEvent(
                    RouteProcessId: Id,
                    StopId: stop.Id,
                    LocationId: stop.LocationId,
                    WasteItemIds: itemIds,
                    OccurredOn: stop.CompletedAt!.Value));
                break;

            case StopOperationType.CustodyTransfer:
                // Only change of custody, no relocation
                AddDomainEvent(new ResidueTransferTriggeredEvent(
                    RouteProcessId: Id,
                    StopId: stop.Id,
                    FromPartyId: DriverId,
                    ToPartyId: stop.ResponsiblePartyId
                        ?? throw new RouteProcessDomainException("CustodyTransfer stop requires a ResponsiblePartyId."),
                    WasteItemIds: itemIds,
                    OccurredOn: stop.CompletedAt!.Value));
                break;
        }

        // Auto-complete route if all stops are done
        if (_stops.All(s => s.IsCompleted))
            CompleteRoute();
    }

    public void Cancel(string reason)
    {
        if (Status == RouteStatus.Completed)
            throw new RouteProcessDomainException("Cannot cancel a completed route.");

        if (Status == RouteStatus.Cancelled)
            throw new RouteProcessDomainException("Route is already cancelled.");

        Status = RouteStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;

        AddDomainEvent(new RouteProcessCancelledEvent(Id, reason, CancelledAt.Value));
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private void CompleteRoute()
    {
        Status = RouteStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new RouteProcessCompletedEvent(Id, VehicleId, CompletedAt.Value));
    }

    private void EnsureStatus(RouteStatus expected, string errorMessage)
    {
        if (Status != expected)
            throw new RouteProcessDomainException(errorMessage);
    }
}
