namespace Gresst.Domain.RouteProcesses;

/// <summary>
/// Type of operation performed at a stop within a Transport route process.
/// Maps to domain OperationTypes (Relocation, Transfer, Storage) when the stop is completed.
/// </summary>
public enum StopOperationType
{
    /// <summary>Residues loaded onto vehicle → triggers Relocation.</summary>
    Pickup = 1,

    /// <summary>Residues unloaded at destination → triggers Relocation + Transfer.</summary>
    Delivery,

    /// <summary>Temporary storage during route → triggers Storage.</summary>
    IntermediateStorage,

    /// <summary>Change of custody only, no physical move → triggers Transfer.</summary>
    CustodyTransfer
}
