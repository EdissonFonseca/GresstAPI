namespace Gresst.Application.DTOs;

public class SupplyDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CategoryUnitId { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string? ParentSupplyId { get; set; }
    public string? ParentSupplyName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSupplyDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CategoryUnitId { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = false;
    /// <summary>
    /// Parent supply ID (for hierarchical structures). Optional.
    /// </summary>
    public string? ParentSupplyId { get; set; }
}

public class UpdateSupplyDto
{
    public string Id { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CategoryUnitId { get; set; }
    public bool? IsPublic { get; set; }
    /// <summary>
    /// Parent supply ID (for hierarchical structures). Optional.
    /// To clear the parent, send null or empty string.
    /// </summary>
    public string? ParentSupplyId { get; set; }
    public bool? IsActive { get; set; }
}

