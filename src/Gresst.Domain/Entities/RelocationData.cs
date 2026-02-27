namespace Gresst.Domain.Entities;

/// <summary>
/// Data for OperationType.Relocation — residue moves from one location to another.
/// </summary>
public class RelocationData : OperationData
{
    /// <summary>Origin location identifier (facility, vehicle, etc.).</summary>
    public string FromLocationId { get; }

    /// <summary>Destination location identifier (facility, vehicle, etc.).</summary>
    public string ToLocationId { get; }

    /// <summary>Vehicle involved in the relocation, when applicable.</summary>
    public Guid? VehicleId { get; }

    /// <summary>
    /// Optional list of waste item identifiers affected by this relocation.
    /// When populated, it provides a direct link from the operation to the
    /// specific WasteItem instances moved.
    /// </summary>
    public IReadOnlyCollection<string> WasteItemIds { get; }

    public RelocationData(
        string fromLocationId,
        string toLocationId,
        Guid? vehicleId = null,
        IEnumerable<string>? wasteItemIds = null)
    {
        FromLocationId = fromLocationId;
        ToLocationId = toLocationId;
        VehicleId = vehicleId;
        WasteItemIds = (wasteItemIds ?? Array.Empty<string>()).ToArray();
    }
}
