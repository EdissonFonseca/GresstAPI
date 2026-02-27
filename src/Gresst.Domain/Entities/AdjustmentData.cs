namespace Gresst.Domain.Entities;

/// <summary>
/// Data for OperationType.Adjustment — correction of recorded quantity.
/// </summary>
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