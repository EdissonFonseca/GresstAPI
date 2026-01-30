namespace Gresst.Application.DTOs;

public class RouteDto
{
    public string Id { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RouteType { get; set; } = string.Empty;
    public string? VehicleId { get; set; }
    public string? VehiclePlate { get; set; }
    public string? DriverId { get; set; }
    public string? DriverName { get; set; }
    public string? Schedule { get; set; }
    public decimal? EstimatedDuration { get; set; }
    public decimal? EstimatedDistance { get; set; }
    public bool IsActive { get; set; }
    public List<RouteStopDto> Stops { get; set; } = new();
}

public class RouteStopDto
{
    public string Id { get; set; } = string.Empty;
    public int Sequence { get; set; }
    public string? LocationId { get; set; }
    public string? LocationName { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public string? Instructions { get; set; }
    public decimal? EstimatedTime { get; set; }
}

public class CreateRouteDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RouteType { get; set; } = string.Empty;
    public string? VehicleId { get; set; }
    public string? DriverId { get; set; }
    public string? Schedule { get; set; }
    public decimal? EstimatedDuration { get; set; }
    public decimal? EstimatedDistance { get; set; }
    public List<CreateRouteStopDto> Stops { get; set; } = new();
}

public class CreateRouteStopDto
{
    public int Sequence { get; set; }
    public string? LocationId { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? PersonId { get; set; }
    public string? FacilityId { get; set; }
    public string? Instructions { get; set; }
    public decimal? EstimatedTime { get; set; }
}

public class UpdateRouteDto
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? RouteType { get; set; }
    public string? VehicleId { get; set; }
    public string? DriverId { get; set; }
    public string? Schedule { get; set; }
    public decimal? EstimatedDuration { get; set; }
    public decimal? EstimatedDistance { get; set; }
    public bool? IsActive { get; set; }
}

public class UpdateRouteStopDto
{
    public string RouteId { get; set; } = string.Empty;
    public string? FacilityId { get; set; }
    public int? Sequence { get; set; }
    public string? LocationId { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? PersonId { get; set; }
    public string? Instructions { get; set; }
    public decimal? EstimatedTime { get; set; }
}

