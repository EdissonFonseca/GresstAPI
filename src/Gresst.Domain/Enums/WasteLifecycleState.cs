namespace Gresst.Domain.Enums;

/// <summary>
/// Waste lifecycle states per waste_management_states.docx.
/// Status = objective fact (where the waste is, what already happened). Persisted on the entity.
/// </summary>
public enum WasteLifecycleState
{
    // ----- Pre-recolección (before physical receipt) -----
    Generated = 1,
    CollectionRequested = 2,
    CollectionConfirmed = 3,
    InTransit = 4,

    // ----- En instalación (under gestor custody) -----
    Received = 10,
    InTemporaryStorage = 11,
    InTreatment = 12,
    Treated = 13,
    InTransfer = 14,
    Transferred = 15,
    InDisposal = 16,
    Disposed = 17,
    Closed = 18,

    // ----- Transformación -----
    InTransformation = 20,
    Transformed = 21,
    Originated = 22,

    // ----- Excepciones -----
    Rejected = 30,
    Observed = 31,
    Retained = 32,
    Expired = 33,
    Cancelled = 34
}
