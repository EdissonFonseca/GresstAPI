namespace Gresst.Domain.Enums;

/// <summary>
/// Request (solicitud) flow stage — process-oriented, per waste_management_states.docx and operational flow.
/// Do not confuse with DB column IdEtapa; mapping from persistence is done in Infrastructure.
/// </summary>
public enum RequestFlowStage
{
    /// <summary>Initial / not yet in transport.</summary>
    Initial = 1,

    /// <summary>Validation.</summary>
    Validation = 2,

    /// <summary>Transport (collection, in transit).</summary>
    Transport = 3,

    /// <summary>Reception at destination.</summary>
    Reception = 4,

    /// <summary>Processing / treatment.</summary>
    Processing = 5,

    /// <summary>Finalization.</summary>
    Finalization = 6
}
