using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Facility service with user-level data segmentation
/// Automatically filters facilities based on current user's assignments
/// </summary>
public class FacilityService : IFacilityService
{
    private readonly IRepository<Facility> _facilityRepository;
    private readonly IDataSegmentationService _segmentationService;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    // Role codes
    private const string CLIENT_ROLE_CODE = "CL";
    private const string PROVIDER_ROLE_CODE = "PR";

    public FacilityService(
        IRepository<Facility> facilityRepository,
        IDataSegmentationService segmentationService,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _facilityRepository = facilityRepository;
        _segmentationService = segmentationService;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Get all facilities - FILTERED by current user assignments
    /// </summary>
    public async Task<IEnumerable<FacilityDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Obtener IDs de facilities asignados al usuario actual
        var userFacilityIds = await _segmentationService.GetUserFacilityIdsAsync(cancellationToken);
        
        // Si es admin, devolver todos
        if (await _segmentationService.CurrentUserIsAdminAsync(cancellationToken))
        {
            var allFacilities = await _facilityRepository.GetAllAsync(cancellationToken);
            return allFacilities.Select(MapToDto).ToList();
        }

        // Usuario normal: solo sus facilities asignados
        var facilities = await _facilityRepository.FindAsync(
            f => userFacilityIds.Contains(f.Id),
            cancellationToken);

        return facilities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Get facility by ID - VERIFIES user has access
    /// </summary>
    public async Task<FacilityDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // Verificar acceso del usuario
        if (!await _segmentationService.UserHasAccessToFacilityAsync(id, cancellationToken))
        {
            return null; // Usuario no tiene acceso
        }

        var facility = await _facilityRepository.GetByIdAsync(id, cancellationToken);
        if (facility == null)
            return null;

        return MapToDto(facility);
    }

    /// <summary>
    /// Get all facilities for admin (no user filtering)
    /// </summary>
    public async Task<IEnumerable<FacilityDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default)
    {
        var facilities = await _facilityRepository.GetAllAsync(cancellationToken);
        return facilities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Get facilities by account - also filtered by user
    /// </summary>
    public async Task<IEnumerable<FacilityDto>> GetAllByAccountAsync(string accountId, CancellationToken cancellationToken = default)
    {
        // Obtener IDs de facilities asignados al usuario actual
        var userFacilityIds = await _segmentationService.GetUserFacilityIdsAsync(cancellationToken);

        var facilities = await _facilityRepository.FindAsync(
            f => f.AccountId == accountId && userFacilityIds.Contains(f.Id),
            cancellationToken);

        return facilities.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Get Account Person ID (persona de la cuenta)
    /// </summary>
    private async Task<Guid> GetAccountPersonIdAsync(CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || account.PersonId == Guid.Empty)
            throw new InvalidOperationException("Account or Account Person not found");
        return account.PersonId;
    }

    /// <summary>
    /// Get facilities for Account Person (persona de la cuenta)
    /// </summary>
    public async Task<IEnumerable<FacilityDto>> GetAccountPersonFacilitiesAsync(CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await GetByPersonAsync(accountPersonId, cancellationToken);
    }

    /// <summary>
    /// Create facility for Account Person (persona de la cuenta)
    /// </summary>
    public async Task<FacilityDto> CreateAccountPersonFacilityAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        dto.PersonId = accountPersonId;
        return await CreateAsync(dto, cancellationToken);
    }

    /// <summary>
    /// Get facilities for a Provider
    /// </summary>
    public async Task<IEnumerable<FacilityDto>> GetProviderFacilitiesAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        // Verify it's a provider (could add validation here)
        return await GetByPersonAsync(providerId, cancellationToken);
    }

    /// <summary>
    /// Create facility for a Provider
    /// </summary>
    public async Task<FacilityDto> CreateProviderFacilityAsync(Guid providerId, CreateFacilityDto dto, CancellationToken cancellationToken = default)
    {
        dto.PersonId = providerId;
        return await CreateAsync(dto, cancellationToken);
    }

    /// <summary>
    /// Get facilities for a Client
    /// </summary>
    public async Task<IEnumerable<FacilityDto>> GetClientFacilitiesAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        // Verify it's a client (could add validation here)
        return await GetByPersonAsync(clientId, cancellationToken);
    }

    /// <summary>
    /// Create facility for a Client
    /// </summary>
    public async Task<FacilityDto> CreateClientFacilityAsync(Guid clientId, CreateFacilityDto dto, CancellationToken cancellationToken = default)
    {
        dto.PersonId = clientId;
        return await CreateAsync(dto, cancellationToken);
    }

    public async Task<FacilityDto> CreateAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default)
    {
        // If PersonId is not provided, use Account Person (persona de la cuenta)
        var personId = dto.PersonId ?? await GetAccountPersonIdAsync(cancellationToken);

        var facility = new Facility
        {
            Id = string.Empty,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            FacilityType = dto.FacilityType,
            Address = dto.Address,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            PersonId = personId,
            CanCollect = dto.CanCollect,
            CanStore = dto.CanStore,
            CanDispose = dto.CanDispose,
            CanTreat = dto.CanTreat,
            CanReceive = dto.CanReceive,
            CanDeliver = dto.CanDeliver,
            MaxCapacity = dto.MaxCapacity,
            CapacityUnit = dto.CapacityUnit,
            CurrentCapacity = 0,
            ParentFacilityId = dto.ParentFacilityId,
            IsVirtual = dto.IsVirtual,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _facilityRepository.AddAsync(facility, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(facility);
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

    public async Task<FacilityDto?> UpdateAsync(UpdateFacilityDto dto, CancellationToken cancellationToken = default)
    {
        // Verificar acceso del usuario
        if (!await _segmentationService.UserHasAccessToFacilityAsync(dto.Id, cancellationToken))
        {
            return null; // Usuario no tiene acceso
        }

        var facility = await _facilityRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (facility == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(dto.Name))
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
        
        // Hierarchical structure
        if (dto.ParentFacilityId.HasValue)
        {
            // If Guid.Empty is provided, clear the parent (set to null)
            if (dto.ParentFacilityId.Value == Guid.Empty)
                facility.ParentFacilityId = null;
            else
                facility.ParentFacilityId = dto.ParentFacilityId.Value;
        }
        
        if (dto.IsVirtual.HasValue)
            facility.IsVirtual = dto.IsVirtual.Value;

        facility.UpdatedAt = DateTime.UtcNow;

        await _facilityRepository.UpdateAsync(facility, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(facility);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        // Verificar acceso del usuario
        if (!await _segmentationService.UserHasAccessToFacilityAsync(id, cancellationToken))
        {
            return false; // Usuario no tiene acceso
        }

        var facility = await _facilityRepository.GetByIdAsync(id, cancellationToken);
        if (facility == null)
            return false;

        await _facilityRepository.DeleteAsync(facility, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
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
            PersonName = facility.Person?.Name,
            CanCollect = facility.CanCollect,
            CanStore = facility.CanStore,
            CanDispose = facility.CanDispose,
            CanTreat = facility.CanTreat,
            CanReceive = facility.CanReceive,
            CanDeliver = facility.CanDeliver,
            MaxCapacity = facility.MaxCapacity,
            CapacityUnit = facility.CapacityUnit,
            CurrentCapacity = facility.CurrentCapacity,
            ParentFacilityId = facility.ParentFacilityId,
            ParentFacilityName = facility.ParentFacility?.Name,
            IsVirtual = facility.IsVirtual,
            CreatedAt = facility.CreatedAt,
            UpdatedAt = facility.UpdatedAt,
            IsActive = facility.IsActive
        };
    }
}
