public class TransferWasteCommand
{
    public Guid WasteId { get; init; }
    public Guid FromOwnerId { get; init; }
    public Guid ToOwnerId { get; init; }
    public Guid? ProcessId { get; init; }
}