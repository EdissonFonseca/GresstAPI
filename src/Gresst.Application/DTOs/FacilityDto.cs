using NetTopologySuite.Geometries;

namespace Gresst.Application.DTOs;

public class    FacilityDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? LocalityId { get; set; }
    public Point? Location { get; set; }
    public List<FacilityType>? Types { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Reference { get; set; }
    public bool IsActive { get; set; }
    public List<FacilityDto> Facilities { get; set; } = new List<FacilityDto>();
    public List<WasteTypeDto> WasteTypes { get; set; } = new List<WasteTypeDto>();
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
    public string? PersonId { get; set; }
    
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
    
    // Hierarchical structure
    /// <summary>
    /// Parent facility ID (for hierarchical structures). Optional.
    /// </summary>
    public string? ParentFacilityId { get; set; }
    
    /// <summary>
    /// Whether this facility is virtual (e.g., for vehicles)
    /// </summary>
    public bool IsVirtual { get; set; } = false;
}

public class UpdateFacilityDto
{
    public string Id { get; set; } = string.Empty;
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
    
    // Hierarchical structure
    /// <summary>
    /// Parent facility ID (for hierarchical structures). Optional.
    /// </summary>
    public string? ParentFacilityId { get; set; }
    
    /// <summary>
    /// Whether this facility is virtual (e.g., for vehicles)
    /// </summary>
    public bool? IsVirtual { get; set; }
}

