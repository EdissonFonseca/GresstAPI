namespace Gresst.Application.DTOs;

public class SupplyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CategoryUnitId { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public Guid? ParentSupplyId { get; set; }
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
    public Guid? ParentSupplyId { get; set; }
}

public class UpdateSupplyDto
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? CategoryUnitId { get; set; }
    public bool? IsPublic { get; set; }
    /// <summary>
    /// Parent supply ID (for hierarchical structures). Optional.
    /// To clear the parent, send Guid.Empty.
    /// </summary>
    public Guid? ParentSupplyId { get; set; }
    public bool? IsActive { get; set; }
}

