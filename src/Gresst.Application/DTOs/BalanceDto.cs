namespace Gresst.Application.DTOs;

public class BalanceDto
{
    public Guid Id { get; set; }
    public Guid? PersonId { get; set; }
    public string? PersonName { get; set; }
    public Guid? FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public Guid WasteTypeId { get; set; }
    public string WasteTypeName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal QuantityGenerated { get; set; }
    public decimal QuantityInTransit { get; set; }
    public decimal QuantityStored { get; set; }
    public decimal QuantityDisposed { get; set; }
    public decimal QuantityTreated { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class InventoryQueryDto
{
    public Guid? PersonId { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? WasteTypeId { get; set; }
}

