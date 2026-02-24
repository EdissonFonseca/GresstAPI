namespace Gresst.Application.DTOs;

public class WasteTypeDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ClassificationId { get; set; }
    public string? ClassificationName { get; set; }
    public bool IsHazardous { get; set; }
    public bool RequiresSpecialHandling { get; set; }
    public string? PhysicalState { get; set; }
    public string? TreatmentId { get; set; }
    public string? TreatmentName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWasteClassDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ClassificationId { get; set; }
    public bool IsHazardous { get; set; }
    public bool RequiresSpecialHandling { get; set; }
    public string? PhysicalState { get; set; }
    public string? TreatmentId { get; set; }
}

public class UpdateWasteClassDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ClassificationId { get; set; }
    public bool? IsHazardous { get; set; }
    public bool? RequiresSpecialHandling { get; set; }
    public string? PhysicalState { get; set; }
    public string? TreatmentId { get; set; }
    public bool? IsActive { get; set; }
}

public class PersonWasteClassDto
{
    public string Id { get; set; } = string.Empty;
    public string PersonId { get; set; } = string.Empty;
    public string? PersonName { get; set; }
    public string WasteClassId { get; set; } = string.Empty;
    public string? WasteClassName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePersonWasteClassDto
{
    public string WasteClassId { get; set; } = string.Empty;
}

public class UpdatePersonWasteClassDto
{
    public string PersonId { get; set; } = string.Empty;
    public string WasteClassId { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
}

