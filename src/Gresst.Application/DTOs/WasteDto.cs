using Gresst.Domain.Enums;

namespace Gresst.Application.DTOs;

public class WasteDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid WasteClassId { get; set; }
    public string WasteClassName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid GeneratorId { get; set; }
    public string GeneratorName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public Guid? CurrentLocationId { get; set; }
    public string? CurrentLocationName { get; set; }
    public Guid? CurrentFacilityId { get; set; }
    public string? CurrentFacilityName { get; set; }
    public bool IsHazardous { get; set; }
    public bool IsAvailableInBank { get; set; }
    public decimal? BankPrice { get; set; }
}

public class CreateWasteDto
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid WasteClassId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public Guid GeneratorId { get; set; }
    public Guid? CurrentLocationId { get; set; }
    public Guid? CurrentFacilityId { get; set; }
    public Guid? PackagingId { get; set; }
    public bool IsHazardous { get; set; }
    public string? BatchNumber { get; set; }
    public string? ContainerNumber { get; set; }
}

public class UpdateWasteDto
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public decimal? Quantity { get; set; }
    public Guid? CurrentLocationId { get; set; }
    public Guid? CurrentFacilityId { get; set; }
    public bool? IsAvailableInBank { get; set; }
    public string? BankDescription { get; set; }
    public decimal? BankPrice { get; set; }
}

