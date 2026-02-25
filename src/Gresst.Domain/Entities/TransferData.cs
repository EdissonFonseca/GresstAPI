public class TransferData : OperationData
{
    public Guid FromOwnerId { get; }
    public Guid ToOwnerId { get; }

    public TransferData(Guid fromOwnerId, Guid toOwnerId)
    {
        FromOwnerId = fromOwnerId;
        ToOwnerId = toOwnerId;
    }
}