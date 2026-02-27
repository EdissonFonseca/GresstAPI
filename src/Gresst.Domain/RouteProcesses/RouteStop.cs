namespace Gresst.Domain.RouteProcesses;

/// <summary>
/// A stop within a RouteProcess. Lives inside the RouteProcess aggregate boundary.
/// </summary>
public class RouteStop
{
    public Guid Id { get; private set; }
    public Guid RouteProcessId { get; private set; }
    public string LocationId { get; private set; } = null!;
    public int Order { get; private set; }
    public StopOperationType OperationType { get; private set; }

    /// <summary>The party (generator, transporter, receiver) responsible at this stop.</summary>
    public Guid? ResponsiblePartyId { get; private set; }

    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    // For EF Core
    private RouteStop() => LocationId = null!;

    internal RouteStop(
        Guid routeProcessId,
        string locationId,
        int order,
        StopOperationType operationType,
        Guid? responsiblePartyId = null)
    {
        Id = Guid.NewGuid();
        RouteProcessId = routeProcessId;
        LocationId = locationId;
        Order = order;
        OperationType = operationType;
        ResponsiblePartyId = responsiblePartyId;
        IsCompleted = false;
    }

    internal void Complete(string? notes = null)
    {
        if (IsCompleted)
            throw new InvalidOperationException($"Stop {Id} is already completed.");

        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        Notes = notes;
    }
}
