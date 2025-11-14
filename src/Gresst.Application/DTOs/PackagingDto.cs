namespace Gresst.Application.DTOs;

public class PackagingDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PackagingType { get; set; } = string.Empty; // Drum, Bag, Container, Tank, etc.
    public decimal? Capacity { get; set; }
    public string? CapacityUnit { get; set; }
    public bool IsReusable { get; set; }
    public string? Material { get; set; }
    public string? UNPackagingCode { get; set; }
    public Guid? ParentPackagingId { get; set; }
    public string? ParentPackagingName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePackagingDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PackagingType { get; set; } = string.Empty;
    public decimal? Capacity { get; set; }
    public string? CapacityUnit { get; set; }
    public bool IsReusable { get; set; }
    public string? Material { get; set; }
    public string? UNPackagingCode { get; set; }
    /// <summary>
    /// Parent packaging ID (for hierarchical structures). Optional.
    /// </summary>
    public Guid? ParentPackagingId { get; set; }
}

public class UpdatePackagingDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? PackagingType { get; set; }
    public decimal? Capacity { get; set; }
    public string? CapacityUnit { get; set; }
    public bool? IsReusable { get; set; }
    public string? Material { get; set; }
    public string? UNPackagingCode { get; set; }
    /// <summary>
    /// Parent packaging ID (for hierarchical structures). Optional.
    /// To clear the parent, send Guid.Empty.
    /// </summary>
    public Guid? ParentPackagingId { get; set; }
    public bool? IsActive { get; set; }
}

