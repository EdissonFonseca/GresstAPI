using Gresst.Domain.Enums;

namespace Gresst.Application.DTOs;

public class WasteDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string WasteClassId { get; set; } = string.Empty;
    public string WasteClassName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string GeneratorId { get; set; } = string.Empty;
    public string GeneratorName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string? CurrentLocationId { get; set; }
    public string? CurrentLocationName { get; set; }
    public string? CurrentFacilityId { get; set; }
    public string? CurrentFacilityName { get; set; }
    public bool IsHazardous { get; set; }
    public bool IsAvailableInBank { get; set; }
    public decimal? BankPrice { get; set; }
}

public class CreateWasteDto
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string WasteClassId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public string GeneratorId { get; set; } = string.Empty;
    public string? CurrentLocationId { get; set; }
    public string? CurrentFacilityId { get; set; }
    public string? PackagingId { get; set; }
    public bool IsHazardous { get; set; }
    public string? BatchNumber { get; set; }
    public string? ContainerNumber { get; set; }
}

public class UpdateWasteDto
{
    public string Id { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Quantity { get; set; }
    public string? CurrentLocationId { get; set; }
    public string? CurrentFacilityId { get; set; }
    public bool? IsAvailableInBank { get; set; }
    public string? BankDescription { get; set; }
    public decimal? BankPrice { get; set; }
}

