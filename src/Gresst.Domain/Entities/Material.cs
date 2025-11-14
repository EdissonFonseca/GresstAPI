using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Material composition of wastes
/// </summary>
public class Material : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Properties
    public bool IsRecyclable { get; set; }
    public bool IsHazardous { get; set; }
    public string? Category { get; set; } // Metal, Plastic, Glass, Organic, etc.
    
    // Waste Type relationship
    public Guid? WasteTypeId { get; set; }
    public virtual WasteType? WasteType { get; set; }
    
    // Navigation properties
    public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();
    public virtual ICollection<PersonMaterial> Persons { get; set; } = new List<PersonMaterial>();
}

