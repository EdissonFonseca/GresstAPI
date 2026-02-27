using Gresst.Application.RouteProcesses;

namespace Gresst.GraphQL.RouteProcesses.Payloads;

/// <summary>
/// GraphQL payload for route process mutations.
/// </summary>
public class RouteProcessPayload
{
    public bool IsSuccess { get; private set; }
    public RouteProcessDto? RouteProcess { get; private set; }
    public string? Error { get; private set; }

    public static RouteProcessPayload Success(RouteProcessDto routeProcess) => new()
    {
        IsSuccess = true,
        RouteProcess = routeProcess
    };

    public static RouteProcessPayload Fail(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}
