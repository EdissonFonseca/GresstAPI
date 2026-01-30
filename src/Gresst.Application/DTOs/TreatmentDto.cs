namespace Gresst.Application.DTOs;

public class TreatmentDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public string? ServiceName { get; set; }
    public string? ProcessDescription { get; set; }
    public decimal? EstimatedDuration { get; set; }
    public string? ApplicableWasteClasses { get; set; }
    public bool ProducesNewWaste { get; set; }
    public string? ResultingWasteClasses { get; set; }
    public string? WasteClassId { get; set; }
    public string? WasteClassName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTreatmentDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string ServiceId { get; set; } = string.Empty;
    public string? ProcessDescription { get; set; }
    public decimal? EstimatedDuration { get; set; }
    public string? ApplicableWasteClasses { get; set; }
    public bool ProducesNewWaste { get; set; }
    public string? ResultingWasteClasses { get; set; }
    public string? WasteClassId { get; set; }
}

public class UpdateTreatmentDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? ServiceId { get; set; }
    public string? ProcessDescription { get; set; }
    public decimal? EstimatedDuration { get; set; }
    public string? ApplicableWasteClasses { get; set; }
    public bool? ProducesNewWaste { get; set; }
    public string? ResultingWasteClasses { get; set; }
    public string? WasteClassId { get; set; }
    public bool? IsActive { get; set; }
}

public class PersonTreatmentDto
{
    public string Id { get; set; } = string.Empty;
    public string PersonId { get; set; } = string.Empty;
    public string? PersonName { get; set; }
    public string TreatmentId { get; set; } = string.Empty;
    public string? TreatmentName { get; set; }
    public bool IsManaged { get; set; }
    public bool CanTransfer { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePersonTreatmentDto
{
    public string TreatmentId { get; set; } = string.Empty;
    public bool IsManaged { get; set; } = true;
    public bool CanTransfer { get; set; } = false;
}

public class UpdatePersonTreatmentDto
{
    public string PersonId { get; set; } = string.Empty;
    public string TreatmentId { get; set; } = string.Empty;
    public bool? IsManaged { get; set; }
    public bool? CanTransfer { get; set; }
    public bool? IsActive { get; set; }
}

