using Gresst.Domain.RouteProcesses;

namespace Gresst.Application.RouteProcesses;

/// <summary>
/// DTO for RouteProcess (Transport process) returned by commands and GraphQL.
/// </summary>
public class RouteProcessDto
{
    public Guid Id { get; init; }
    public Guid VehicleId { get; init; }
    public Guid DriverId { get; init; }
    public RouteStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    public string? CancellationReason { get; init; }
    public IReadOnlyList<RouteStopDto> Stops { get; init; } = Array.Empty<RouteStopDto>();
    public int ProgressPercent { get; init; }
}

public class RouteStopDto
{
    public Guid Id { get; init; }
    public string LocationId { get; init; } = string.Empty;
    public int Order { get; init; }
    public StopOperationType OperationType { get; init; }
    public Guid? ResponsiblePartyId { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? Notes { get; init; }
}
