namespace Gresst.Domain.Entities;

/// <summary>
/// Data for OperationType.Transfer — change of legal holder/owner.
/// </summary>
public class TransferData : OperationData
{
    public Guid FromOwnerId { get; }
    public Guid ToOwnerId { get; }

    /// <summary>
    /// Optional list of waste item identifiers whose custody changed.
    /// </summary>
    public IReadOnlyCollection<string> WasteItemIds { get; }

    public TransferData(
        Guid fromOwnerId,
        Guid toOwnerId,
        IEnumerable<string>? wasteItemIds = null)
    {
        FromOwnerId = fromOwnerId;
        ToOwnerId = toOwnerId;
        WasteItemIds = (wasteItemIds ?? Array.Empty<string>()).ToArray();
    }
}