public class WasteOperation
{
    public string Id { get; }
    public OperationType Type { get; }
    public DateTime OccurredAt { get; }
    public Guid? ProcessId { get; }
    public OperationData Data { get; }

    public WasteOperation(OperationType type, OperationData data, Guid? processId = null)
    {
        Id = Guid.NewGuid().ToString();
        Type = type;
        Data = data;
        ProcessId = processId;
        OccurredAt = DateTime.UtcNow;
    }
}