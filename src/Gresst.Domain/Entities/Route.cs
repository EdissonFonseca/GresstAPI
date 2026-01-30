using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Routes for collection/transport planning
/// </summary>
public class Route : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Route Type
    public string RouteType { get; set; } = string.Empty; // Collection, Transport, Delivery
    
    // Assignment
    public string? VehicleId { get; set; }
    public virtual Vehicle? Vehicle { get; set; }
    
    public string? DriverId { get; set; }
    public virtual Person? Driver { get; set; }
    
    // Scheduling
    public string? Schedule { get; set; } // JSON: Days of week, frequency
    public decimal? EstimatedDuration { get; set; } // in hours
    public decimal? EstimatedDistance { get; set; } // in km
    
    // Status
    public new bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<RouteStop> Stops { get; set; } = new List<RouteStop>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

/// <summary>
/// Stops in a route
/// </summary>
public class RouteStop : BaseEntity
{
    public string RouteId { get; set; } = string.Empty;
    public virtual Route Route { get; set; } = null!;
    
    public int Sequence { get; set; }
    
    // Location
    public string? LocationId { get; set; }
    public virtual Location? Location { get; set; }
    
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    // Person/Facility at stop
    public string? PersonId { get; set; }
    public virtual Person? Person { get; set; }
    
    public string? FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    
    // Details
    public string? Instructions { get; set; }
    public decimal? EstimatedTime { get; set; } // minutes
}

