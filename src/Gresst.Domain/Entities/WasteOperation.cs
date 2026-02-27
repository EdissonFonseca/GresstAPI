using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

public class WasteOperation
{
    public string Id { get; }
    public OperationType Type { get; }
    public DateTime OccurredAt { get; }
    public Guid? ProcessId { get; }
    public OperationData Data { get; }

    public WasteOperation(OperationType type, OperationData data, Guid? processId = null)
        : this(type, data, processId, DateTime.UtcNow)
    {
    }

    public WasteOperation(OperationType type, OperationData data, Guid? processId, DateTime occurredAt)
    {
        Id = Guid.NewGuid().ToString();
        Type = type;
        Data = data;
        ProcessId = processId;
        OccurredAt = occurredAt;
    }
}