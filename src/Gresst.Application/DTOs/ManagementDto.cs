using Gresst.Domain.Enums;

namespace Gresst.Application.DTOs;

public class ManagementDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; }
    public string WasteId { get; set; } = string.Empty;
    public string WasteCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string ExecutedById { get; set; } = string.Empty;
    public string ExecutedByName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateManagementDto
{
    public ManagementType Type { get; set; }
    public string WasteId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public string ExecutedById { get; set; } = string.Empty;
    public string? OriginLocationId { get; set; }
    public string? OriginFacilityId { get; set; }
    public string? DestinationLocationId { get; set; }
    public string? DestinationFacilityId { get; set; }
    public string? OrderId { get; set; }
    public string? VehicleId { get; set; }
    public string? TreatmentId { get; set; }
    public string? Notes { get; set; }
}

// Specific operation DTOs
public class CollectWasteDto
{
    public string WasteId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string CollectorId { get; set; } = string.Empty;
    public string? VehicleId { get; set; }
    public string? OriginLocationId { get; set; }
    public string? Notes { get; set; }
}

public class TransportWasteDto
{
    public string WasteId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string TransporterId { get; set; } = string.Empty;
    public string VehicleId { get; set; } = string.Empty;
    public string OriginFacilityId { get; set; } = string.Empty;
    public string DestinationFacilityId { get; set; } = string.Empty;
    public string? RouteId { get; set; }
    public string? Notes { get; set; }
}

public class DisposeWasteDto
{
    public string WasteId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string DisposerId { get; set; } = string.Empty;
    public string FacilityId { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class TransformWasteDto
{
    public string SourceWasteId { get; set; } = string.Empty;
    public decimal SourceQuantity { get; set; }
    public string ResultWasteClassId { get; set; } = string.Empty;
    public decimal ResultQuantity { get; set; }
    public TransformationType Type { get; set; }
    public string? TreatmentId { get; set; }
    public string PerformedById { get; set; } = string.Empty;
    public string? FacilityId { get; set; }
    public string? Description { get; set; }
}

public class StoreWasteDto
{
    public string WasteId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string LocationId { get; set; } = string.Empty;
    public string? FacilityId { get; set; }
    public bool IsPermanent { get; set; }
    public string? Notes { get; set; }
}

