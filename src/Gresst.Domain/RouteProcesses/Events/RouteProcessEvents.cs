using Gresst.Domain.Common;

namespace Gresst.Domain.RouteProcesses.Events;

// ─────────────────────────────────────────────────────────────
// Route lifecycle events
// ─────────────────────────────────────────────────────────────

public record RouteProcessCreatedEvent(
    Guid RouteProcessId,
    Guid VehicleId,
    Guid DriverId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record RouteProcessStartedEvent(
    Guid RouteProcessId,
    Guid VehicleId,
    Guid DriverId,
    DateTime OccurredOn
) : IDomainEvent;

public record RouteProcessCompletedEvent(
    Guid RouteProcessId,
    Guid VehicleId,
    DateTime OccurredOn
) : IDomainEvent;

public record RouteProcessCancelledEvent(
    Guid RouteProcessId,
    string Reason,
    DateTime OccurredOn
) : IDomainEvent;

// ─────────────────────────────────────────────────────────────
// Operation-trigger events
// These are consumed by downstream handlers that create the
// actual Operation records (Relocation, Transfer, Storage).
// ─────────────────────────────────────────────────────────────

/// <summary>
/// Fired on Pickup and Delivery stops.
/// Handler creates an OperationType.Relocation record.
/// </summary>
public record ResidueRelocationTriggeredEvent(
    Guid RouteProcessId,
    Guid StopId,
    string LocationId,
    Guid VehicleId,
    IReadOnlyCollection<string> WasteItemIds,
    DateTime OccurredOn
) : IDomainEvent;

/// <summary>
/// Fired on Delivery and CustodyTransfer stops.
/// Handler creates an OperationType.Transfer record.
/// </summary>
public record ResidueTransferTriggeredEvent(
    Guid RouteProcessId,
    Guid StopId,
    Guid FromPartyId,
    Guid ToPartyId,
    IReadOnlyCollection<string> WasteItemIds,
    DateTime OccurredOn
) : IDomainEvent;

/// <summary>
/// Fired on IntermediateStorage stops.
/// Handler creates an OperationType.Storage record.
/// </summary>
public record ResidueStorageTriggeredEvent(
    Guid RouteProcessId,
    Guid StopId,
    string LocationId,
    IReadOnlyCollection<string> WasteItemIds,
    DateTime OccurredOn
) : IDomainEvent;
