using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty; // Truck, Van, Tanker, etc.
    public string? Model { get; set; }
    public string? Make { get; set; }
    public int? Year { get; set; }
    
    // Owner
    public Guid PersonId { get; set; }
    public virtual Person Person { get; set; } = null!;
    
    // Capacity
    public decimal? MaxCapacity { get; set; }
    public string? CapacityUnit { get; set; }
    
    // Properties
    public bool IsAvailable { get; set; } = true;
    public string? SpecialEquipment { get; set; }
    
    /// <summary>
    /// Virtual facility associated with this vehicle
    /// Allows inventory movements to/from vehicles as if they were facilities
    /// </summary>
    public Guid? VirtualFacilityId { get; set; }
    public virtual Facility? VirtualFacility { get; set; }
    
    // Navigation properties
    public virtual ICollection<Management> Managements { get; set; } = new List<Management>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

