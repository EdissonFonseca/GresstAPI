using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// International waste classification codes (UN, LER, Y-Code, A-Code)
/// </summary>
public class Classification : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Classification System Type
    public string ClassificationSystem { get; set; } = string.Empty; // UN, LER, Y-Code, A-Code, etc.
    
    // Hierarchy support
    public Guid? ParentId { get; set; }
    public virtual Classification? Parent { get; set; }
    public virtual ICollection<Classification> Children { get; set; } = new List<Classification>();
    
    // Properties
    public bool IsHazardous { get; set; }
    public string? RegulationReference { get; set; }
    
    // Navigation properties
    public virtual ICollection<WasteClass> WasteClasses { get; set; } = new List<WasteClass>();
}

