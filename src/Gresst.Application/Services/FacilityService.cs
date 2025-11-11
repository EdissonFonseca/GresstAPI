using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

public class FacilityService : IFacilityService
{
    private readonly IRepository<Facility> _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FacilityService(
        IRepository<Facility> facilityRepository,
        IUnitOfWork unitOfWork)
    {
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FacilityDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var facility = await _facilityRepository.GetByIdAsync(id, cancellationToken);
        if (facility == null)
            throw new KeyNotFoundException($"Facility with ID {id} not found");

        return MapToDto(facility);
    }

    public async Task<IEnumerable<FacilityDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var facilities = await _facilityRepository.GetAllAsync(cancellationToken);
        return facilities.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<FacilityDto>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        var facilities = await _facilityRepository.FindAsync(
            f => f.PersonId == personId, 
            cancellationToken);
        
        return facilities.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<FacilityDto>> GetByTypeAsync(string facilityType, CancellationToken cancellationToken = default)
    {
        var facilities = await _facilityRepository.FindAsync(
            f => f.FacilityType == facilityType, 
            cancellationToken);
        
        return facilities.Select(MapToDto).ToList();
    }

    public async Task<FacilityDto> CreateAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default)
    {
        var facility = new Facility
        {
            Id = Guid.NewGuid(),
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            FacilityType = dto.FacilityType,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            PersonId = dto.PersonId,
            CanCollect = dto.CanCollect,
            CanStore = dto.CanStore,
            CanDispose = dto.CanDispose,
            CanTreat = dto.CanTreat,
            CanReceive = dto.CanReceive,
            CanDeliver = dto.CanDeliver,
            MaxCapacity = dto.MaxCapacity,
            CapacityUnit = dto.CapacityUnit,
            CurrentCapacity = 0,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _facilityRepository.AddAsync(facility, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(facility);
    }

    public async Task<FacilityDto> UpdateAsync(UpdateFacilityDto dto, CancellationToken cancellationToken = default)
    {
        var facility = await _facilityRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (facility == null)
            throw new KeyNotFoundException($"Facility with ID {dto.Id} not found");

        // Update only provided fields
        if (dto.Name != null)
            facility.Name = dto.Name;
        if (dto.Description != null)
            facility.Description = dto.Description;
        if (dto.Address != null)
            facility.Address = dto.Address;
        if (dto.Latitude.HasValue)
            facility.Latitude = dto.Latitude;
        if (dto.Longitude.HasValue)
            facility.Longitude = dto.Longitude;
        
        // Capabilities
        if (dto.CanCollect.HasValue)
            facility.CanCollect = dto.CanCollect.Value;
        if (dto.CanStore.HasValue)
            facility.CanStore = dto.CanStore.Value;
        if (dto.CanDispose.HasValue)
            facility.CanDispose = dto.CanDispose.Value;
        if (dto.CanTreat.HasValue)
            facility.CanTreat = dto.CanTreat.Value;
        if (dto.CanReceive.HasValue)
            facility.CanReceive = dto.CanReceive.Value;
        if (dto.CanDeliver.HasValue)
            facility.CanDeliver = dto.CanDeliver.Value;
        
        // Capacity
        if (dto.MaxCapacity.HasValue)
            facility.MaxCapacity = dto.MaxCapacity;
        if (dto.CurrentCapacity.HasValue)
            facility.CurrentCapacity = dto.CurrentCapacity;

        facility.UpdatedAt = DateTime.UtcNow;

        await _facilityRepository.UpdateAsync(facility, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(facility);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var facility = await _facilityRepository.GetByIdAsync(id, cancellationToken);
        if (facility == null)
            throw new KeyNotFoundException($"Facility with ID {id} not found");

        await _facilityRepository.DeleteAsync(facility, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private FacilityDto MapToDto(Facility facility)
    {
        return new FacilityDto
        {
            Id = facility.Id,
            Code = facility.Code,
            Name = facility.Name,
            Description = facility.Description,
            FacilityType = facility.FacilityType,
            Address = facility.Address,
            Latitude = facility.Latitude,
            Longitude = facility.Longitude,
            PersonId = facility.PersonId,
            CanCollect = facility.CanCollect,
            CanStore = facility.CanStore,
            CanDispose = facility.CanDispose,
            CanTreat = facility.CanTreat,
            CanReceive = facility.CanReceive,
            CanDeliver = facility.CanDeliver,
            MaxCapacity = facility.MaxCapacity,
            CapacityUnit = facility.CapacityUnit,
            CurrentCapacity = facility.CurrentCapacity,
            CreatedAt = facility.CreatedAt,
            UpdatedAt = facility.UpdatedAt,
            IsActive = facility.IsActive
        };
    }
}

