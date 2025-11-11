using Gresst.Domain.Enums;

namespace Gresst.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Description { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid? WasteId { get; set; }
    public Guid? WasteTypeId { get; set; }
    public string WasteTypeName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class CreateOrderDto
{
    public OrderType Type { get; set; }
    public Guid ProviderId { get; set; }
    public Guid ClientId { get; set; }
    public Guid? RequestId { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public string? Description { get; set; }
    public decimal? EstimatedCost { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid? RouteId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public Guid? WasteId { get; set; }
    public Guid? WasteTypeId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public string? Notes { get; set; }
}

