namespace Gresst.Application.DTOs;

public class RouteDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RouteType { get; set; } = string.Empty;
    public Guid? VehicleId { get; set; }
    public string? VehiclePlate { get; set; }
    public Guid? DriverId { get; set; }
    public string? DriverName { get; set; }
    public string? Schedule { get; set; }
    public decimal? EstimatedDuration { get; set; }
    public decimal? EstimatedDistance { get; set; }
    public bool IsActive { get; set; }
    public List<RouteStopDto> Stops { get; set; } = new();
}

public class RouteStopDto
{
    public Guid Id { get; set; }
    public int Sequence { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Guid? PersonId { get; set; }
    public string? PersonName { get; set; }
    public Guid? FacilityId { get; set; }
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
    public Guid? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public string? Schedule { get; set; }
    public decimal? EstimatedDuration { get; set; }
    public decimal? EstimatedDistance { get; set; }
    public List<CreateRouteStopDto> Stops { get; set; } = new();
}

public class CreateRouteStopDto
{
    public int Sequence { get; set; }
    public Guid? LocationId { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Guid? PersonId { get; set; }
    public Guid? FacilityId { get; set; }
    public string? Instructions { get; set; }
    public decimal? EstimatedTime { get; set; }
}

public class UpdateRouteDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? RouteType { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? DriverId { get; set; }
    public string? Schedule { get; set; }
    public decimal? EstimatedDuration { get; set; }
    public decimal? EstimatedDistance { get; set; }
    public bool? IsActive { get; set; }
}

public class UpdateRouteStopDto
{
    public Guid RouteId { get; set; }
    public Guid? FacilityId { get; set; }
    public int? Sequence { get; set; }
    public Guid? LocationId { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public Guid? PersonId { get; set; }
    public string? Instructions { get; set; }
    public decimal? EstimatedTime { get; set; }
}

