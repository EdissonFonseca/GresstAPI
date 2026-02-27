namespace Gresst.GraphQL.RouteProcesses.Inputs;

public class CompleteRouteStopInput
{
    public Guid RouteProcessId { get; set; }
    public Guid StopId { get; set; }
    public string? Notes { get; set; }
}
