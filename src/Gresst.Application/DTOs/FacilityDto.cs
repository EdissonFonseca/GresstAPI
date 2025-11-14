namespace Gresst.Application.DTOs;

public class FacilityDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FacilityType { get; set; } = string.Empty;
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    
    // Capabilities
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
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateFacilityDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FacilityType { get; set; } = string.Empty;
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    /// <summary>
    /// PersonId is optional. If not provided, uses the Account Person (persona de la cuenta).
    /// </summary>
    public Guid? PersonId { get; set; }
    
    // Capabilities
    public bool CanCollect { get; set; }
    public bool CanStore { get; set; }
    public bool CanDispose { get; set; }
    public bool CanTreat { get; set; }
    public bool CanReceive { get; set; }
    public bool CanDeliver { get; set; }
    
    // Capacity
    public decimal? MaxCapacity { get; set; }
    public string? CapacityUnit { get; set; }
}

public class UpdateFacilityDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Capabilities
    public bool? CanCollect { get; set; }
    public bool? CanStore { get; set; }
    public bool? CanDispose { get; set; }
    public bool? CanTreat { get; set; }
    public bool? CanReceive { get; set; }
    public bool? CanDeliver { get; set; }
    
    // Capacity
    public decimal? MaxCapacity { get; set; }
    public decimal? CurrentCapacity { get; set; }
}

