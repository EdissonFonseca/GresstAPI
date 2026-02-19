using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Supply (Insumo) - Supplies/inputs used in logistics operations
/// Represents consumables, materials, or resources used in waste management processes
/// </summary>
public class Supply : BaseEntity
{
    public string? Description { get; set; }
    
    public string CategoryUnitId { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether this supply is public (visible to all accounts)
    /// </summary>
    public bool IsPublic { get; set; }
    
    /// <summary>
    /// Parent supply (for hierarchical categorization)
    /// </summary>
    public string? ParentId { get; set; }
    
}

