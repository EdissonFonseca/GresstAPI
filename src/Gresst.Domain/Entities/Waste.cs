using Gresst.Domain.Entities;

public class Waste : AggregateRoot
{
    public string Id { get; private set; }
    public WasteType WasteType { get; init; }
    public decimal Quantity { get; private set; }
    public WasteStatus Status { get; private set; }
    public string CurrentOwnerId { get; private set; }
    public string CurrentLocationId { get; private set; }

    private readonly List<WasteOperation> _operations = new();
    public IReadOnlyCollection<WasteOperation> Operations => _operations;
    public void ApplyOperation(WasteOperation operation)
    {
        // domain rules here
        _operations.Add(operation);
    }

    private Waste() { 
        Id = Guid.NewGuid().ToString();
        CurrentLocationId = Guid.NewGuid().ToString();
        CurrentOwnerId = Guid.NewGuid().ToString();
        WasteType = new WasteType();
    }
    
    public Waste(WasteType wasteType, decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        Id = Guid.NewGuid().ToString();
        CurrentLocationId = Guid.NewGuid().ToString();
        CurrentOwnerId = Guid.NewGuid().ToString();
        WasteType = wasteType ?? throw new ArgumentNullException(nameof(wasteType));
        Quantity = quantity;
        Status = WasteStatus.Generated;
    }
}