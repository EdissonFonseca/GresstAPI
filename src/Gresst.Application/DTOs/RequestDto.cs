using Gresst.Domain.Enums;

namespace Gresst.Application.DTOs;

public class RequestDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public Guid? ProviderId { get; set; }
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
    public Guid Id { get; set; }
    public Guid WasteClassId { get; set; }
    public string WasteClassName { get; set; } = string.Empty;
    public decimal EstimatedQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateRequestDto
{
    public Guid RequesterId { get; set; }
    public Guid? ProviderId { get; set; }
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
    public Guid WasteClassId { get; set; }
    public decimal EstimatedQuantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public string? Description { get; set; }
}

