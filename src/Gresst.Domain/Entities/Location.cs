using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Dynamic hierarchical storage structure (Sites, Facilities, Plants, Warehouses, Containers, etc.)
/// </summary>
public class Location : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string LocationType { get; set; } = string.Empty; // Site, Warehouse, Container, Zone, etc.
    
    // Geographic information
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Hierarchy support - allows flexible levels
    public string? ParentLocationId { get; set; }
    public virtual Location? ParentLocation { get; set; }
    public virtual ICollection<Location> ChildLocations { get; set; } = new List<Location>();
    
    // Capacity
    public decimal? MaxCapacity { get; set; }
    public string? CapacityUnit { get; set; }
    
    // Association with Person/Facility
    public string? PersonId { get; set; }
    public virtual Person? Person { get; set; }
    
    public string? FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    
    // Navigation properties
    public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();
    public virtual ICollection<PersonContact> PersonContacts { get; set; } = new List<PersonContact>();
}

