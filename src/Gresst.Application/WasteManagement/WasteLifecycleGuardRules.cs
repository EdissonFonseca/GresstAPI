using Gresst.Domain.Enums;

namespace Gresst.Application.WasteManagement;

/// <summary>
/// Guard rules (preconditions) per waste_management_states.docx §5.
/// See docs/waste_management_states_reference.md.
/// Must be satisfied before executing a transition; evaluated in the domain/application layer.
/// </summary>
public static class WasteLifecycleGuardRules
{
    /// <summary>
    /// Precondition for transitioning to InTransit: active signed transport manifest must exist.
    /// </summary>
    public const string ToInTransit = "Transport manifest active and signed.";

    /// <summary>
    /// Precondition for transitioning to Received: carrier confirmed delivery; quantity within configured tolerance (±% by classification).
    /// </summary>
    public const string ToReceived = "Carrier confirmed delivery; quantity within tolerance.";

    /// <summary>
    /// Precondition for transitioning to InTreatment: planned_destination must be set; waste_treatment record in PLANNED state must exist.
    /// </summary>
    public const string ToInTreatment = "planned_destination set; waste_treatment PLANNED exists.";

    /// <summary>
    /// Precondition for transitioning to InTransfer: transfer document exists; destination facility enabled for that waste type.
    /// </summary>
    public const string ToInTransfer = "Transfer document exists; destination enabled for waste type.";

    /// <summary>
    /// Precondition for transitioning to InTransformation: all input waste_items are IN_TEMPORARY_STORAGE, TREATED or RECEIVED.
    /// </summary>
    public const string ToInTransformation = "All input items in IN_TEMPORARY_STORAGE, TREATED or RECEIVED.";

    /// <summary>
    /// Precondition for transitioning to Closed: closure or final certificate document exists per waste classification.
    /// </summary>
    public const string ToClosed = "Closure or final certificate document exists.";

    /// <summary>
    /// Precondition for Cancelled: waste has not physically arrived at any facility (never passed through RECEIVED).
    /// </summary>
    public const string ToCancelled = "Waste has not reached RECEIVED.";

    /// <summary>
    /// Returns the guard rule description for a given target state (for validation messages).
    /// </summary>
    public static string? GetGuardRuleForTarget(WasteLifecycleState target)
    {
        return target switch
        {
            WasteLifecycleState.InTransit => ToInTransit,
            WasteLifecycleState.Received => ToReceived,
            WasteLifecycleState.InTreatment => ToInTreatment,
            WasteLifecycleState.InTransfer => ToInTransfer,
            WasteLifecycleState.InTransformation => ToInTransformation,
            WasteLifecycleState.Closed => ToClosed,
            WasteLifecycleState.Cancelled => ToCancelled,
            _ => null
        };
    }
}
