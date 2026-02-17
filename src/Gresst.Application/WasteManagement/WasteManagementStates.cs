using Gresst.Domain.Enums;

namespace Gresst.Application.WasteManagement;

/// <summary>
/// Process-oriented state definitions aligned with waste_management_states.docx.
/// See docs/waste_management_states_reference.md and project root waste_management_states.docx.
/// No persistence codes (IdEtapa, IdFase, IdEstado) — those and their mapping live in Infrastructure.
/// </summary>
public static class WasteManagementStates
{
    // ----- Waste lifecycle (waste_management_states.docx §2) -----
    // Canonical: Gresst.Domain.Enums.WasteLifecycleState, Gresst.Domain.Common.WasteLifecyclePhase

    /// <summary>
    /// Pre-reception phase: GENERATED → COLLECTION_REQUESTED → COLLECTION_CONFIRMED → IN_TRANSIT.
    /// </summary>
    public static readonly WasteLifecycleState[] LifecyclePreRecoleccion =
    {
        WasteLifecycleState.Generated,
        WasteLifecycleState.CollectionRequested,
        WasteLifecycleState.CollectionConfirmed,
        WasteLifecycleState.InTransit
    };

    /// <summary>
    /// Exception states: REJECTED, OBSERVED, RETAINED, EXPIRED, CANCELLED.
    /// </summary>
    public static readonly WasteLifecycleState[] LifecycleExcepcion =
    {
        WasteLifecycleState.Rejected,
        WasteLifecycleState.Observed,
        WasteLifecycleState.Retained,
        WasteLifecycleState.Expired,
        WasteLifecycleState.Cancelled
    };

    // ----- Legacy WasteStatus (entity field; map to WasteLifecycleState when migrating) -----

    public const string WasteGroupPreReception = "PreReception";
    public const string WasteGroupPostReception = "PostReception";

    public static readonly WasteStatus[] WastePreReceptionStatuses = { WasteStatus.Generated, WasteStatus.InTransit };
    public static readonly WasteStatus[] WastePostReceptionStatuses =
    {
        WasteStatus.Stored,
        WasteStatus.InTreatment,
        WasteStatus.Disposed,
        WasteStatus.Transformed,
        WasteStatus.Delivered,
        WasteStatus.Sold,
        WasteStatus.Reused
    };

    // ----- Request flow: use Domain enums only -----
    // RequestFlowStage, RequestFlowPhase (Gresst.Domain.Enums).
    // Filter defaults (estado codes, service ID) come from IRequestFilterDefaults in Infrastructure.
}
