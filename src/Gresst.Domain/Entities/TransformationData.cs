using Gresst.Domain.Entities;

public class TransformationData : OperationData
{
    public WasteType NewWasteType { get; }
    public decimal OutputQuantity { get; }

    public TransformationData(WasteType newWasteType, decimal outputQuantity)
    {
        NewWasteType = newWasteType;
        OutputQuantity = outputQuantity;
    }
}