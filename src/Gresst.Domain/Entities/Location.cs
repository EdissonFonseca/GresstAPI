using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Dynamic hierarchical storage structure (Sites, Facilities, Plants, Warehouses, Containers, etc.)
/// </summary>
public class Location : BaseEntity
{
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Hierarchy support - allows flexible levels
    public string? ParentLocationId { get; set; }
    public virtual ICollection<Location> ChildLocations { get; set; } = new List<Location>();
    }

