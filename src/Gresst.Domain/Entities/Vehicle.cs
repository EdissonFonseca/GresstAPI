using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty; // Truck, Van, Tanker, etc.
    public string? Model { get; set; }
    public string? Make { get; set; }
    public int? Year { get; set; }
        
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
    public string? VirtualFacilityId { get; set; }
}

