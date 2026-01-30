using Gresst.Domain.Enums;

namespace Gresst.Application.DTOs;

public class RequestDto
{
    public string Id { get; set; } = string.Empty;
    public string RequestNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RequesterId { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string? ProviderId { get; set; }
    public string? ProviderName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string[] ServicesRequested { get; set; } = Array.Empty<string>();
    public DateTime RequestedDate { get; set; }
    public DateTime? RequiredByDate { get; set; }
    public decimal? EstimatedCost { get; set; }
    public decimal? AgreedCost { get; set; }
    public List<RequestItemDto> Items { get; set; } = new();
}

public class RequestItemDto
{
    public string Id { get; set; } = string.Empty;
    public string WasteClassId { get; set; } = string.Empty;
    public string WasteClassName { get; set; } = string.Empty;
    public decimal EstimatedQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateRequestDto
{
    public string RequesterId { get; set; } = string.Empty;
    public string? ProviderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string[] ServicesRequested { get; set; } = Array.Empty<string>();
    public DateTime? RequiredByDate { get; set; }
    public string? PickupAddress { get; set; }
    public string? DeliveryAddress { get; set; }
    public List<CreateRequestItemDto> Items { get; set; } = new();
}

public class CreateRequestItemDto
{
    public string WasteClassId { get; set; } = string.Empty;
    public decimal EstimatedQuantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public string? Description { get; set; }
}

