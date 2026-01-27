using Gresst.Domain.Enums;

namespace Gresst.Application.DTOs;

public class ManagementDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime ExecutedAt { get; set; }
    public Guid WasteId { get; set; }
    public string WasteCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public Guid ExecutedById { get; set; }
    public string ExecutedByName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateManagementDto
{
    public ManagementType Type { get; set; }
    public Guid WasteId { get; set; }
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public Guid ExecutedById { get; set; }
    public Guid? OriginLocationId { get; set; }
    public Guid? OriginFacilityId { get; set; }
    public Guid? DestinationLocationId { get; set; }
    public Guid? DestinationFacilityId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? TreatmentId { get; set; }
    public string? Notes { get; set; }
}

// Specific operation DTOs
public class CollectWasteDto
{
    public Guid WasteId { get; set; }
    public decimal Quantity { get; set; }
    public Guid CollectorId { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? OriginLocationId { get; set; }
    public string? Notes { get; set; }
}

public class TransportWasteDto
{
    public Guid WasteId { get; set; }
    public decimal Quantity { get; set; }
    public Guid TransporterId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid OriginFacilityId { get; set; }
    public Guid DestinationFacilityId { get; set; }
    public Guid? RouteId { get; set; }
    public string? Notes { get; set; }
}

public class DisposeWasteDto
{
    public Guid WasteId { get; set; }
    public decimal Quantity { get; set; }
    public Guid DisposerId { get; set; }
    public Guid FacilityId { get; set; }
    public string? Notes { get; set; }
}

public class TransformWasteDto
{
    public Guid SourceWasteId { get; set; }
    public decimal SourceQuantity { get; set; }
    public Guid ResultWasteClassId { get; set; }
    public decimal ResultQuantity { get; set; }
    public TransformationType Type { get; set; }
    public Guid? TreatmentId { get; set; }
    public Guid PerformedById { get; set; }
    public Guid? FacilityId { get; set; }
    public string? Description { get; set; }
}

public class StoreWasteDto
{
    public Guid WasteId { get; set; }
    public decimal Quantity { get; set; }
    public Guid LocationId { get; set; }
    public Guid? FacilityId { get; set; }
    public bool IsPermanent { get; set; }
    public string? Notes { get; set; }
}

