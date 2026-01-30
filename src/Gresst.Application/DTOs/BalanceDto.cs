namespace Gresst.Application.DTOs;

public class BalanceDto
{
    public string Id { get; set; } = string.Empty;
    public string? PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public string? LocationId { get; set; }
    public string? LocationName { get; set; }
    public string WasteClassId { get; set; } = string.Empty;
    public string WasteClassName { get; set; } = string.Empty;
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
    public string? PersonId { get; set; }
    public string? FacilityId { get; set; }
    public string? LocationId { get; set; }
    public string? WasteClassId { get; set; }
}

