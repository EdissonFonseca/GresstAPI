namespace Gresst.Domain.Entities;

/// <summary>
/// Marker base type for strongly-typed payloads attached to a WasteOperation.
/// Each concrete subclass should capture the domain-relevant dimensions for a
/// specific OperationType (e.g. RelocationData, TransferData, StorageData).
/// </summary>
public abstract class OperationData
{
    /// <summary>
    /// Optional identifier for a batch or logical grouping of residues affected
    /// by this operation. This complements item-level identifiers when present.
    /// </summary>
    public string? BatchId { get; set; }
}