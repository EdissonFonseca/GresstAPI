using Gresst.Application.DTOs;
using Gresst.Domain.Enums;

namespace Gresst.Application.WasteManagement;

/// <summary>
/// Waste management business rules aligned with waste_management_states.docx.
/// See docs/waste_management_states_reference.md.
/// - Waste: lifecycle transitions (§4) in WasteLifecycleTransitions; guard rules (§5) in WasteLifecycleGuardRules.
/// - Request flow: predicates use process Stage/Phase (Domain enums), not persistence codes.
/// </summary>
public static class WasteManagementRules
{
    // ----- Waste lifecycle (doc §4): use WasteLifecycleState + WasteLifecycleTransitions -----

    /// <summary>
    /// Valid target states from current state (per doc §4).
    /// </summary>
    public static IReadOnlyList<WasteLifecycleState> GetAllowedTargetStates(WasteLifecycleState current) =>
        WasteLifecycleTransitions.GetAllowedTargetStates(current);

    /// <summary>
    /// True if transition from current to target is allowed (does not check guard rules).
    /// </summary>
    public static bool IsTransitionAllowed(WasteLifecycleState current, WasteLifecycleState target) =>
        WasteLifecycleTransitions.IsTransitionAllowed(current, target);

    // ----- Legacy: ManagementType actions from WasteStatus -----

    public static IReadOnlyList<ManagementType> GetAllowedActionsForWaste(WasteStatus currentStatus)
    {
        return currentStatus switch
        {
            WasteStatus.Generated => new[] { ManagementType.Collect, ManagementType.Transport },
            WasteStatus.InTransit => new[] { ManagementType.Receive, ManagementType.Transport },
            WasteStatus.Stored => new[] { ManagementType.Treat, ManagementType.Dispose, ManagementType.Store, ManagementType.StorePermanent, ManagementType.Transform, ManagementType.Deliver, ManagementType.Sell, ManagementType.Transfer },
            WasteStatus.InTreatment => new[] { ManagementType.Store, ManagementType.Dispose, ManagementType.Transform, ManagementType.Deliver },
            WasteStatus.Disposed => Array.Empty<ManagementType>(),
            WasteStatus.Transformed => new[] { ManagementType.Store, ManagementType.Treat, ManagementType.Dispose, ManagementType.Deliver, ManagementType.Sell },
            WasteStatus.Delivered => Array.Empty<ManagementType>(),
            WasteStatus.Sold => Array.Empty<ManagementType>(),
            WasteStatus.Reused => new[] { ManagementType.Store, ManagementType.Deliver, ManagementType.Sell },
            _ => Array.Empty<ManagementType>()
        };
    }

    public static bool IsWastePreReception(WasteStatus status) =>
        status == WasteStatus.Generated || status == WasteStatus.InTransit;

    // ----- Request flow: process-oriented (Stage / Phase from Domain enums) -----

    /// <summary>
    /// Included in mobile transport view: Transport, Reception, or Processing/Initial, or Finalization/Finalization.
    /// </summary>
    public static bool IsIncludedInMobileTransport(SolicitudWithDetailsDto s)
    {
        if (s.Item == 0) return true;
        return s.Stage == RequestFlowStage.Transport
               || s.Stage == RequestFlowStage.Reception
               || (s.Stage == RequestFlowStage.Processing && s.Phase == RequestFlowPhase.Initial)
               || (s.Stage == RequestFlowStage.Finalization && s.Phase == RequestFlowPhase.Finalization);
    }

    /// <summary>
    /// Pending collection: Initial, or Processing/Initial, or Transport with Initial or Planning.
    /// </summary>
    public static bool IsPendingCollection(SolicitudWithDetailsDto s)
    {
        if (s.Item == 0) return true;
        return s.Stage == RequestFlowStage.Initial
               || (s.Stage == RequestFlowStage.Processing && s.Phase == RequestFlowPhase.Initial)
               || (s.Stage == RequestFlowStage.Transport && (s.Phase == RequestFlowPhase.Initial || s.Phase == RequestFlowPhase.Planning));
    }

    /// <summary>
    /// Pending reception: Transport, or Reception not yet finalized.
    /// </summary>
    public static bool IsPendingReception(SolicitudWithDetailsDto s)
    {
        if (s.Item == 0) return false;
        return s.Stage == RequestFlowStage.Transport
               || (s.Stage == RequestFlowStage.Reception && s.Phase != RequestFlowPhase.Finalization);
    }

    /// <summary>
    /// Pending treatment: Reception finalized or Processing not yet finalized.
    /// </summary>
    public static bool IsPendingTreatment(SolicitudWithDetailsDto s)
    {
        if (s.Item == 0) return false;
        return (s.Stage == RequestFlowStage.Reception && s.Phase == RequestFlowPhase.Finalization)
               || (s.Stage == RequestFlowStage.Processing && s.Phase != RequestFlowPhase.Finalization);
    }
}
