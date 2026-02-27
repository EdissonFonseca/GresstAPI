namespace Gresst.Domain.RouteProcesses;

/// <summary>
/// Thrown when a route process invariant is violated (e.g. invalid state transition, missing required data).
/// </summary>
public class RouteProcessDomainException : Exception
{
    public RouteProcessDomainException(string message) : base(message) { }

    public RouteProcessDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
