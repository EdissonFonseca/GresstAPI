namespace Gresst.Domain.Enums;

/// <summary>
/// Phase within a request flow stage — process-oriented, per waste_management_states.docx.
/// Do not confuse with DB column IdFase; mapping from persistence is done in Infrastructure.
/// </summary>
public enum RequestFlowPhase
{
    /// <summary>Initial / start of stage.</summary>
    Initial = 1,

    /// <summary>Planning.</summary>
    Planning = 2,

    /// <summary>Execution.</summary>
    Execution = 3,

    /// <summary>Certification.</summary>
    Certification = 4,

    /// <summary>Finalization / completed for this stage.</summary>
    Finalization = 5
}
