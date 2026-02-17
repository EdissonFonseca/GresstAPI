using Gresst.Domain.Enums;

namespace Gresst.Application.WasteManagement;

/// <summary>
/// Valid state transitions per waste_management_states.docx §4 + additional rules.
/// See docs/waste_management_states_reference.md. Do not add transitions without updating the document.
/// - Transfer: allowed from any post-reception state (Received, InTemporaryStorage, InTreatment, Treated, InDisposal, Observed).
/// Any transition not listed must be rejected by the domain layer with a business exception.
/// </summary>
public static class WasteLifecycleTransitions
{
    /// <summary>
    /// States from which transfer is allowed (any stage after received).
    /// </summary>
    public static readonly WasteLifecycleState[] StatesAllowingTransfer =
    {
        WasteLifecycleState.Received,
        WasteLifecycleState.InTemporaryStorage,
        WasteLifecycleState.InTreatment,
        WasteLifecycleState.Treated,
        WasteLifecycleState.InDisposal,
        WasteLifecycleState.Observed
    };

    private static readonly Dictionary<WasteLifecycleState, WasteLifecycleState[]> AllowedTargets = new()
    {
        [WasteLifecycleState.Generated] = new[] { WasteLifecycleState.CollectionRequested, WasteLifecycleState.Cancelled },
        [WasteLifecycleState.CollectionRequested] = new[] { WasteLifecycleState.CollectionConfirmed, WasteLifecycleState.Rejected, WasteLifecycleState.Cancelled },
        [WasteLifecycleState.CollectionConfirmed] = new[] { WasteLifecycleState.InTransit, WasteLifecycleState.Cancelled },
        [WasteLifecycleState.InTransit] = new[] { WasteLifecycleState.Received, WasteLifecycleState.Rejected, WasteLifecycleState.InTransformation }, // REPACK only from IN_TRANSIT
        [WasteLifecycleState.Received] = new[] { WasteLifecycleState.InTemporaryStorage, WasteLifecycleState.Observed, WasteLifecycleState.Rejected, WasteLifecycleState.InTransfer },
        [WasteLifecycleState.Observed] = new[] { WasteLifecycleState.InTemporaryStorage, WasteLifecycleState.Rejected, WasteLifecycleState.InTransfer },
        [WasteLifecycleState.InTemporaryStorage] = new[] { WasteLifecycleState.InTreatment, WasteLifecycleState.InTransfer, WasteLifecycleState.InDisposal, WasteLifecycleState.InTransformation, WasteLifecycleState.Expired },
        [WasteLifecycleState.InTreatment] = new[] { WasteLifecycleState.Treated, WasteLifecycleState.InTreatment, WasteLifecycleState.Observed, WasteLifecycleState.InTransfer }, // IN_TREATMENT = next cycle
        [WasteLifecycleState.Treated] = new[] { WasteLifecycleState.InTemporaryStorage, WasteLifecycleState.InTransfer, WasteLifecycleState.InDisposal, WasteLifecycleState.InTransformation, WasteLifecycleState.Closed },
        [WasteLifecycleState.InTransfer] = new[] { WasteLifecycleState.Transferred, WasteLifecycleState.Rejected },
        [WasteLifecycleState.Transferred] = new[] { WasteLifecycleState.Closed },
        [WasteLifecycleState.InDisposal] = new[] { WasteLifecycleState.Disposed, WasteLifecycleState.InTransfer },
        [WasteLifecycleState.Disposed] = new[] { WasteLifecycleState.Closed },
        [WasteLifecycleState.InTransformation] = new[] { WasteLifecycleState.Transformed },
        [WasteLifecycleState.Transformed] = Array.Empty<WasteLifecycleState>(), // terminal
        [WasteLifecycleState.Originated] = new[] { WasteLifecycleState.InTemporaryStorage },
        [WasteLifecycleState.Rejected] = new[] { WasteLifecycleState.CollectionRequested },
        [WasteLifecycleState.Expired] = new[] { WasteLifecycleState.InTreatment, WasteLifecycleState.InDisposal },
        [WasteLifecycleState.Retained] = Array.Empty<WasteLifecycleState>(), // special: can revert to previous state (handled separately)
        [WasteLifecycleState.Cancelled] = Array.Empty<WasteLifecycleState>(),
        [WasteLifecycleState.Closed] = Array.Empty<WasteLifecycleState>()
    };

    /// <summary>
    /// Returns the list of valid target states from the given current state.
    /// Empty for terminal states (Transformed, Closed, Cancelled).
    /// </summary>
    public static IReadOnlyList<WasteLifecycleState> GetAllowedTargetStates(WasteLifecycleState current)
    {
        return AllowedTargets.TryGetValue(current, out var list) ? list : Array.Empty<WasteLifecycleState>();
    }

    /// <summary>
    /// Returns true if the transition from current to target is allowed by the document.
    /// Does not evaluate guard rules (preconditions); those must be checked separately.
    /// </summary>
    public static bool IsTransitionAllowed(WasteLifecycleState current, WasteLifecycleState target)
    {
        var allowed = GetAllowedTargetStates(current);
        return allowed.Contains(target);
    }

    /// <summary>
    /// True if transfer is allowed from this state (any stage after received).
    /// </summary>
    public static bool AllowsTransfer(WasteLifecycleState current) =>
        StatesAllowingTransfer.Contains(current);
}
