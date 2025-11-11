using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Facilities are physical installations like treatment plants, disposal sites, storage facilities
/// </summary>
public class Facility : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FacilityType { get; set; } = string.Empty; // TreatmentPlant, DisposalSite, StorageFacility, TransferStation
    
    // Location
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Owner
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    
    // Capabilities - What operations this facility can perform
    public bool CanCollect { get; set; }
    public bool CanStore { get; set; }
    public bool CanDispose { get; set; }
    public bool CanTreat { get; set; }
    public bool CanReceive { get; set; }
    public bool CanDeliver { get; set; }
    
    // Capacity
    public decimal? MaxCapacity { get; set; }
    public string? CapacityUnit { get; set; }
    public decimal? CurrentCapacity { get; set; }
    
    // Navigation properties
    public virtual ICollection<License> Licenses { get; set; } = new List<License>();
    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
    public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();
}

