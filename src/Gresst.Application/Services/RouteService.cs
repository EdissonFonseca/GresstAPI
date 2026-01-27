using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using DomainRoute = Gresst.Domain.Entities.Route;
using DomainRouteStop = Gresst.Domain.Entities.RouteStop;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing Routes and RouteStops
/// </summary>
public class RouteService : IRouteService
{
    private readonly IRepository<DomainRoute> _routeRepository;
    private readonly IRepository<DomainRouteStop> _routeStopRepository;
    private readonly IRepository<Vehicle>? _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RouteService(
        IRepository<DomainRoute> routeRepository,
        IRepository<DomainRouteStop> routeStopRepository,
        IRepository<Vehicle>? vehicleRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _routeRepository = routeRepository;
        _routeStopRepository = routeStopRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<RouteDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var route = await _routeRepository.GetByIdAsync(id, cancellationToken);
        if (route == null)
            return null;

        return await MapToDtoAsync(route, cancellationToken);
    }

    public async Task<IEnumerable<RouteDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var routes = await _routeRepository.GetAllAsync(cancellationToken);
        var result = new List<RouteDto>();
        
        foreach (var route in routes)
        {
            result.Add(await MapToDtoAsync(route, cancellationToken));
        }
        
        return result;
    }

    public async Task<IEnumerable<RouteDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var routes = await _routeRepository.FindAsync(
            r => r.IsActive,
            cancellationToken);
        
        var result = new List<RouteDto>();
        foreach (var route in routes)
        {
            result.Add(await MapToDtoAsync(route, cancellationToken));
        }
        
        return result;
    }

    public async Task<IEnumerable<RouteDto>> GetByTypeAsync(string routeType, CancellationToken cancellationToken = default)
    {
        var routes = await _routeRepository.FindAsync(
            r => r.RouteType.Equals(routeType, StringComparison.OrdinalIgnoreCase),
            cancellationToken);
        
        var result = new List<RouteDto>();
        foreach (var route in routes)
        {
            result.Add(await MapToDtoAsync(route, cancellationToken));
        }
        
        return result;
    }

    public async Task<IEnumerable<RouteDto>> GetByVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var routes = await _routeRepository.FindAsync(
            r => r.VehicleId == vehicleId,
            cancellationToken);
        
        var result = new List<RouteDto>();
        foreach (var route in routes)
        {
            result.Add(await MapToDtoAsync(route, cancellationToken));
        }
        
        return result;
    }

    public async Task<RouteDto> CreateAsync(CreateRouteDto dto, CancellationToken cancellationToken = default)
    {
        var route = new DomainRoute
        {
            Id = string.Empty,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            RouteType = dto.RouteType,
            VehicleId = dto.VehicleId,
            DriverId = dto.DriverId,
            Schedule = dto.Schedule,
            EstimatedDuration = dto.EstimatedDuration,
            EstimatedDistance = dto.EstimatedDistance,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Load Vehicle if VehicleId is provided
        if (route.VehicleId.HasValue && route.VehicleId.Value != Guid.Empty && _vehicleRepository != null)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(route.VehicleId.Value.ToString(), cancellationToken);
            if (vehicle != null)
            {
                route.Vehicle = vehicle;
            }
        }

        await _routeRepository.AddAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Add stops if provided
        if (dto.Stops != null && dto.Stops.Any())
        {
            foreach (var stopDto in dto.Stops)
            {
                if (stopDto.FacilityId.HasValue)
                {
                    var stop = new DomainRouteStop
                    {
                        Id = string.Empty,
                        RouteId = Guid.Parse(route.Id),
                        Sequence = stopDto.Sequence,
                        FacilityId = stopDto.FacilityId,
                        LocationId = stopDto.LocationId,
                        Address = stopDto.Address,
                        Latitude = stopDto.Latitude,
                        Longitude = stopDto.Longitude,
                        PersonId = stopDto.PersonId,
                        Instructions = stopDto.Instructions,
                        EstimatedTime = stopDto.EstimatedTime,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _routeStopRepository.AddAsync(stop, cancellationToken);
                }
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return await MapToDtoAsync(route, cancellationToken);
    }

    public async Task<RouteDto?> UpdateAsync(UpdateRouteDto dto, CancellationToken cancellationToken = default)
    {
        var route = await _routeRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (route == null)
            return null;

        // Update only provided fields (PATCH-like)
        if (!string.IsNullOrEmpty(dto.Name))
            route.Name = dto.Name;
        if (dto.Description != null)
            route.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.RouteType))
            route.RouteType = dto.RouteType;
        if (dto.VehicleId.HasValue)
        {
            route.VehicleId = dto.VehicleId;
            // Load Vehicle if provided
            if (dto.VehicleId.Value != Guid.Empty && _vehicleRepository != null)
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId.Value.ToString(), cancellationToken);
                if (vehicle != null)
                {
                    route.Vehicle = vehicle;
                }
            }
        }
        if (dto.DriverId.HasValue)
            route.DriverId = dto.DriverId;
        if (dto.Schedule != null)
            route.Schedule = dto.Schedule;
        if (dto.EstimatedDuration.HasValue)
            route.EstimatedDuration = dto.EstimatedDuration;
        if (dto.EstimatedDistance.HasValue)
            route.EstimatedDistance = dto.EstimatedDistance;
        if (dto.IsActive.HasValue)
            route.IsActive = dto.IsActive.Value;

        route.UpdatedAt = DateTime.UtcNow;

        await _routeRepository.UpdateAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(route, cancellationToken);
    }

    public async Task<RouteStopDto> AddStopAsync(Guid routeId, CreateRouteStopDto dto, CancellationToken cancellationToken = default)
    {
        if (!dto.FacilityId.HasValue)
            throw new InvalidOperationException("RouteStop must have a FacilityId");

        var route = await _routeRepository.GetByIdAsync(routeId.ToString(), cancellationToken);
        if (route == null)
            throw new KeyNotFoundException($"Route with ID {routeId} not found");

        var stop = new DomainRouteStop
        {
            Id = string.Empty,
            RouteId = routeId,
            Sequence = dto.Sequence,
            FacilityId = dto.FacilityId,
            LocationId = dto.LocationId,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            PersonId = dto.PersonId,
            Instructions = dto.Instructions,
            EstimatedTime = dto.EstimatedTime,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _routeStopRepository.AddAsync(stop, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapStopToDto(stop);
    }

    public async Task<bool> RemoveStopAsync(Guid routeId, Guid facilityId, CancellationToken cancellationToken = default)
    {
        var stops = await _routeStopRepository.FindAsync(
            rs => rs.RouteId == routeId && rs.FacilityId == facilityId,
            cancellationToken);
        
        var stop = stops.FirstOrDefault();
        if (stop == null)
            return false;

        await _routeStopRepository.DeleteAsync(stop, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<RouteDto> ReorderStopsAsync(Guid routeId, Dictionary<Guid, int> stopSequences, CancellationToken cancellationToken = default)
    {
        var route = await _routeRepository.GetByIdAsync(routeId.ToString(), cancellationToken);
        if (route == null)
            throw new KeyNotFoundException($"Route with ID {routeId} not found");

        var stops = await _routeStopRepository.FindAsync(
            rs => rs.RouteId == routeId,
            cancellationToken);

        foreach (var stop in stops)
        {
            if (stop.FacilityId.HasValue && stopSequences.ContainsKey(stop.FacilityId.Value))
            {
                stop.Sequence = stopSequences[stop.FacilityId.Value];
                stop.UpdatedAt = DateTime.UtcNow;
                await _routeStopRepository.UpdateAsync(stop, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(route, cancellationToken);
    }

    public async Task<RouteDto?> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var route = await _routeRepository.GetByIdAsync(id.ToString(), cancellationToken);
        if (route == null)
            return null;

        route.IsActive = true;
        route.UpdatedAt = DateTime.UtcNow;

        await _routeRepository.UpdateAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(route, cancellationToken);
    }

    public async Task<RouteDto?> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var route = await _routeRepository.GetByIdAsync(id.ToString(), cancellationToken);
        if (route == null)
            return null;

        route.IsActive = false;
        route.UpdatedAt = DateTime.UtcNow;

        await _routeRepository.UpdateAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(route, cancellationToken);
    }

    private async Task<RouteDto> MapToDtoAsync(DomainRoute route, CancellationToken cancellationToken)
    {
        // Load stops for this route (RouteStop.RouteId is Guid, route.Id is string)
        var routeIdGuid = Guid.Parse(route.Id);
        var stops = await _routeStopRepository.FindAsync(
            rs => rs.RouteId == routeIdGuid,
            cancellationToken);

        return new RouteDto
        {
            Id = route.Id,
            Code = route.Code,
            Name = route.Name,
            Description = route.Description,
            RouteType = route.RouteType,
            VehicleId = route.VehicleId,
            VehiclePlate = route.Vehicle?.LicensePlate,
            DriverId = route.DriverId,
            DriverName = route.Driver?.Name,
            Schedule = route.Schedule,
            EstimatedDuration = route.EstimatedDuration,
            EstimatedDistance = route.EstimatedDistance,
            IsActive = route.IsActive,
            Stops = stops.Select(MapStopToDto).OrderBy(s => s.Sequence).ToList()
        };
    }

    private RouteStopDto MapStopToDto(DomainRouteStop stop)
    {
        return new RouteStopDto
        {
            Id = stop.Id,
            Sequence = stop.Sequence,
            LocationId = stop.LocationId,
            LocationName = stop.Location?.Name,
            Address = stop.Address,
            Latitude = stop.Latitude,
            Longitude = stop.Longitude,
            PersonId = stop.PersonId,
            PersonName = stop.Person?.Name,
            FacilityId = stop.FacilityId,
            FacilityName = stop.Facility?.Name,
            Instructions = stop.Instructions,
            EstimatedTime = stop.EstimatedTime
        };
    }
}

