namespace Gresst.Domain.Entities;

/// <summary>
/// Data for OperationType.Storage — residue is held temporarily at a location.
/// </summary>
public class StorageData : OperationData
{
    public string LocationId { get; }

    public StorageData(string locationId)
    {
        LocationId = locationId;
    }
}
