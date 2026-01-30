using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Supply (Insumo) - Supplies/inputs used in logistics operations
/// Represents consumables, materials, or resources used in waste management processes
/// </summary>
public class Supply : BaseEntity
{
    /// <summary>
    /// Supply code or reference
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Supply name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Supply description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Category/Unit identifier
    /// </summary>
    public string CategoryUnitId { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether this supply is public (visible to all accounts)
    /// </summary>
    public bool IsPublic { get; set; }
    
    /// <summary>
    /// Parent supply (for hierarchical categorization)
    /// </summary>
    public string? ParentSupplyId { get; set; }
    public virtual Supply? ParentSupply { get; set; }
    
    /// <summary>
    /// Child supplies (for hierarchical categorization)
    /// </summary>
    public virtual ICollection<Supply> ChildSupplies { get; set; } = new List<Supply>();
    
    /// <summary>
    /// Persons that use/manage this supply
    /// </summary>
    public virtual ICollection<PersonSupply> Persons { get; set; } = new List<PersonSupply>();
}

