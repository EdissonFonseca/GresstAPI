namespace Gresst.Domain.Entities;

/// <summary>
/// Data for OperationType.Storage — residue is held temporarily at a location.
/// </summary>
public class StorageData : OperationData
{
    /// <summary>Location where the residue is being stored.</summary>
    public string LocationId { get; }

    /// <summary>
    /// Optional list of waste item identifiers affected by this storage
    /// operation. When provided, it links the operation directly to the
    /// WasteItem instances being stored.
    /// </summary>
    public IReadOnlyCollection<string> WasteItemIds { get; }

    public StorageData(string locationId, IEnumerable<string>? wasteItemIds = null)
    {
        LocationId = locationId;
        WasteItemIds = (wasteItemIds ?? Array.Empty<string>()).ToArray();
    }
}
