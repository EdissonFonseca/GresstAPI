using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Facilities are physical installations like treatment plants, disposal sites, storage facilities
/// </summary>
public class Facility : BaseEntity
{
    public string? Description { get; set; }
    public FacilityType? Type { get; set; }
    public string? LocationId { get; set; }    
    // Location
    public string? Address { get; set; }
    // Capacity
    public decimal? MaxCapacity { get; set; }
    public string? CapacityUnit { get; set; }
    public decimal? CurrentCapacity { get; set; }
    
    public string? ParentId { get; set; }
    public ICollection<Facility> Nodes { get; set; } = new List<Facility>();    
    public ICollection<License> Licenses { get; set; } = new List<License>();
    public ICollection<WasteType> WasteTypes { get; set; } = new List<WasteType>();
    public ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}

