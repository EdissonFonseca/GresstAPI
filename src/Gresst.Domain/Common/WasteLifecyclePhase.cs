using Gresst.Domain.Enums;

namespace Gresst.Domain.Common;

/// <summary>
/// Lifecycle phases from waste_management_states.docx.
/// </summary>
public static class WasteLifecyclePhase
{
    public const string PreRecoleccion = "PreRecoleccion";
    public const string EnInstalacion = "EnInstalacion";
    public const string Transformacion = "Transformacion";
    public const string Excepcion = "Excepcion";

    public static string GetPhase(WasteLifecycleState state)
    {
        return state switch
        {
            WasteLifecycleState.Generated => PreRecoleccion,
            WasteLifecycleState.CollectionRequested => PreRecoleccion,
            WasteLifecycleState.CollectionConfirmed => PreRecoleccion,
            WasteLifecycleState.InTransit => PreRecoleccion,

            WasteLifecycleState.Received => EnInstalacion,
            WasteLifecycleState.InTemporaryStorage => EnInstalacion,
            WasteLifecycleState.InTreatment => EnInstalacion,
            WasteLifecycleState.Treated => EnInstalacion,
            WasteLifecycleState.InTransfer => EnInstalacion,
            WasteLifecycleState.Transferred => EnInstalacion,
            WasteLifecycleState.InDisposal => EnInstalacion,
            WasteLifecycleState.Disposed => EnInstalacion,
            WasteLifecycleState.Closed => EnInstalacion,

            WasteLifecycleState.InTransformation => Transformacion,
            WasteLifecycleState.Transformed => Transformacion,
            WasteLifecycleState.Originated => Transformacion,

            WasteLifecycleState.Rejected => Excepcion,
            WasteLifecycleState.Observed => Excepcion,
            WasteLifecycleState.Retained => Excepcion,
            WasteLifecycleState.Expired => Excepcion,
            WasteLifecycleState.Cancelled => Excepcion,

            _ => EnInstalacion
        };
    }

    public static bool IsPreReception(WasteLifecycleState state) => GetPhase(state) == PreRecoleccion;
    public static bool IsTerminal(WasteLifecycleState state) =>
        state == WasteLifecycleState.Transformed || state == WasteLifecycleState.Closed || state == WasteLifecycleState.Cancelled;
}
