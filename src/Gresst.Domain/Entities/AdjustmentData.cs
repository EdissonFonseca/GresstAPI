public class AdjustmentData : OperationData
{
    public decimal NewQuantity { get; }
    public string Reason { get; }

    public AdjustmentData(decimal newQuantity, string reason)
    {
        NewQuantity = newQuantity;
        Reason = reason;
    }
}