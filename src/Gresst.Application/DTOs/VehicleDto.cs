namespace Gresst.Application.DTOs;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Make { get; set; }
    public int? Year { get; set; }
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    
    // Capacity
    public decimal? MaxCapacity { get; set; }
    public string? CapacityUnit { get; set; }
    
    // Properties
    public bool IsAvailable { get; set; }
    public string? SpecialEquipment { get; set; }
    
    // Virtual Facility
    public Guid? VirtualFacilityId { get; set; }
    public string? VirtualFacilityName { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateVehicleDto
{
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? Make { get; set; }
    public int? Year { get; set; }
    /// <summary>
    /// PersonId is optional. If not provided, uses the Account Person (persona de la cuenta).
    /// </summary>
    public Guid? PersonId { get; set; }
    
    // Capacity
    public decimal? MaxCapacity { get; set; }
    public string? CapacityUnit { get; set; }
    
    // Properties
    public bool IsAvailable { get; set; } = true;
    public string? SpecialEquipment { get; set; }
    
    // Virtual Facility
    public Guid? VirtualFacilityId { get; set; }
}

public class UpdateVehicleDto
{
    public Guid Id { get; set; }
    public string? LicensePlate { get; set; }
    public string? VehicleType { get; set; }
    public string? Model { get; set; }
    public string? Make { get; set; }
    public int? Year { get; set; }
    
    // Capacity
    public decimal? MaxCapacity { get; set; }
    public string? CapacityUnit { get; set; }
    
    // Properties
    public bool? IsAvailable { get; set; }
    public string? SpecialEquipment { get; set; }
    
    // Virtual Facility
    public Guid? VirtualFacilityId { get; set; }
    
    public bool? IsActive { get; set; }
}

