using Gresst.Domain.Enums;

namespace Gresst.Application.DTOs;

public class OrderDto
{
    public string Id { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Description { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string Id { get; set; } = string.Empty;
    public string? WasteId { get; set; }
    public string? WasteClassId { get; set; }
    public string WasteClassName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class CreateOrderDto
{
    public OrderType Type { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string? RequestId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? Description { get; set; }
    public decimal? EstimatedCost { get; set; }
    public string? VehicleId { get; set; }
    public string? FacilityId { get; set; }
    public string? RouteId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public string? WasteId { get; set; }
    public string? WasteClassId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public string? Notes { get; set; }
}

