namespace Gresst.GraphQL.RouteProcesses.Inputs;

/// <summary>
/// GraphQL input for creating a new transport route process.
/// </summary>
public class CreateRouteProcessInput
{
    public Guid VehicleId { get; set; }
    public Guid DriverId { get; set; }
    public List<CreateRouteStopInputItem> Stops { get; set; } = new();
}

/// <summary>
/// A single stop in the route.
/// </summary>
public class CreateRouteStopInputItem
{
    public string LocationId { get; set; } = string.Empty;
    public StopOperationTypeInput OperationType { get; set; }
    public Guid? ResponsiblePartyId { get; set; }
}

public enum StopOperationTypeInput
{
    Pickup = 1,
    Delivery,
    IntermediateStorage,
    CustodyTransfer
}
