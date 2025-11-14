using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

public class WasteClass : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // International Classifications
    public Guid? ClassificationId { get; set; }
    public virtual Classification? Classification { get; set; }
    
    // Properties
    public bool IsHazardous { get; set; }
    public bool RequiresSpecialHandling { get; set; }
    public string? PhysicalState { get; set; } // Solid, Liquid, Gas, Sludge
    
    // Treatment - One-to-one optional relationship
    public Guid? TreatmentId { get; set; }
    public virtual Treatment? Treatment { get; set; }
    
    // Navigation properties
    public virtual ICollection<Waste> Wastes { get; set; } = new List<Waste>();
    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
    public virtual ICollection<FacilityWasteClass> Facilities { get; set; } = new List<FacilityWasteClass>();
    
    // Persons - Persons (account, provider, client) associated with this waste class
    public virtual ICollection<PersonWasteClass> Persons { get; set; } = new List<PersonWasteClass>();
}

