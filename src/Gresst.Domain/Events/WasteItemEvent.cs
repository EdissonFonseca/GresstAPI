using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Events;

/// <summary>
/// Domain event that captures a state transition for a single waste item.
/// 
/// It is the residue-centric view of what happened, typically derived from one
/// or more process Operations (see WasteOperation and OperationType).
/// </summary>
public class WasteItemEvent : DomainEvent
{
    /// <summary>Previous lifecycle status of the item.</summary>
    public WasteItemStatus FromStatus { get; set; }

    /// <summary>New lifecycle status of the item after the event.</summary>
    public WasteItemStatus ToStatus { get; set; }

    /// <summary>Atomic operation that conceptually produced this transition.</summary>
    public OperationType Operation { get; set; }

    /// <summary>Type of custody/handling change for the item (e.g. pickup, delivery).</summary>
    public HandoverType HandoverType { get; set; }

    /// <summary>
    /// Identifier of the WasteOperation that originated this event, when applicable.
    /// This links the item-level history with the process-level operation log.
    /// </summary>
    public string? OperationId { get; set; }

    /// <summary>
    /// Process identifier (e.g. RouteProcessId) that contained the originating operation.
    /// Useful for tracing the item back to the composite Process instance.
    /// </summary>
    public Guid? ProcessId { get; set; }

    public string? ProcedureId { get; set; }
    public string? OrderId { get; set; }
    public string? FromFacilityId { get; set; }
    public string? ToFacilityId { get; set; }
    public string? FromPartyId { get; set; }
    public string? ToPartyId { get; set; }
}