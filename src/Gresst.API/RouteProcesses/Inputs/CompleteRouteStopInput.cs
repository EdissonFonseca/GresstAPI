namespace Gresst.GraphQL.RouteProcesses.Inputs;

public class CompleteRouteStopInput
{
    public Guid RouteProcessId { get; set; }
    public Guid StopId { get; set; }
    public string? Notes { get; set; }

    /// <summary>
    /// Optional list of waste item identifiers handled at this stop.
    /// The mobile app can send all items for the stop when approving it.
    /// </summary>
    public List<string>? WasteItemIds { get; set; }
}
