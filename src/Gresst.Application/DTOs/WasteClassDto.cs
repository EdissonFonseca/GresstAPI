namespace Gresst.Application.DTOs;

public class WasteClassDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ClassificationId { get; set; }
    public string? ClassificationName { get; set; }
    public bool IsHazardous { get; set; }
    public bool RequiresSpecialHandling { get; set; }
    public string? PhysicalState { get; set; }
    public Guid? TreatmentId { get; set; }
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
    public Guid? ClassificationId { get; set; }
    public bool IsHazardous { get; set; }
    public bool RequiresSpecialHandling { get; set; }
    public string? PhysicalState { get; set; }
    public Guid? TreatmentId { get; set; }
}

public class UpdateWasteClassDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? ClassificationId { get; set; }
    public bool? IsHazardous { get; set; }
    public bool? RequiresSpecialHandling { get; set; }
    public string? PhysicalState { get; set; }
    public Guid? TreatmentId { get; set; }
    public bool? IsActive { get; set; }
}

public class PersonWasteClassDto
{
    public string Id { get; set; } = string.Empty;
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public Guid WasteClassId { get; set; }
    public string? WasteClassName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePersonWasteClassDto
{
    public Guid WasteClassId { get; set; }
}

public class UpdatePersonWasteClassDto
{
    public Guid PersonId { get; set; }
    public Guid WasteClassId { get; set; }
    public bool? IsActive { get; set; }
}

