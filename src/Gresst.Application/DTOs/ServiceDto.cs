namespace Gresst.Application.DTOs;

public class ServiceDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CategoryCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CategoryCode { get; set; }
}

public class UpdateServiceDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CategoryCode { get; set; }
    public bool? IsActive { get; set; }
}

public class PersonServiceDto
{
    public string Id { get; set; } = string.Empty;
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public Guid ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePersonServiceDto
{
    public Guid ServiceId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdatePersonServiceDto
{
    public Guid PersonId { get; set; }
    public Guid ServiceId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; }
}

