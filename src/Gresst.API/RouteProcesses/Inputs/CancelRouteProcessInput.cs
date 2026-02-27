namespace Gresst.GraphQL.RouteProcesses.Inputs;

public class CancelRouteProcessInput
{
    public Guid RouteProcessId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
