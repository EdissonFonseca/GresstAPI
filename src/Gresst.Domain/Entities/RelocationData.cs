namespace Gresst.Domain.Entities;

/// <summary>
/// Data for OperationType.Relocation — residue moves from one location to another.
/// </summary>
public class RelocationData : OperationData
{
    public string FromLocationId { get; }
    public string ToLocationId { get; }
    public Guid? VehicleId { get; }

    public RelocationData(string fromLocationId, string toLocationId, Guid? vehicleId = null)
    {
        FromLocationId = fromLocationId;
        ToLocationId = toLocationId;
        VehicleId = vehicleId;
    }
}
