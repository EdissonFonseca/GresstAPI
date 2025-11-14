using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Vehicle service with user-level data segmentation
/// Automatically filters vehicles based on current user's assignments
/// </summary>
public class VehicleService : IVehicleService
{
    private readonly IRepository<Vehicle> _vehicleRepository;
    private readonly IDataSegmentationService _segmentationService;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    // Role codes
    private const string CLIENT_ROLE_CODE = "CL";
    private const string PROVIDER_ROLE_CODE = "PR";

    public VehicleService(
        IRepository<Vehicle> vehicleRepository,
        IDataSegmentationService segmentationService,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _vehicleRepository = vehicleRepository;
        _segmentationService = segmentationService;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Get all vehicles - FILTERED by current user assignments (with segmentation)
    /// </summary>
    public async Task<IEnumerable<VehicleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Obtener IDs de vehículos asignados al usuario actual
        var userVehicleIds = await _segmentationService.GetUserVehicleIdsAsync(cancellationToken);
        
        // Si es admin, devolver todos
        if (await _segmentationService.CurrentUserIsAdminAsync(cancellationToken))
        {
            var allVehicles = await _vehicleRepository.GetAllAsync(cancellationToken);
            return allVehicles.Select(MapToDto).ToList();
        }

        // Usuario normal: solo sus vehículos asignados
        var vehicles = await _vehicleRepository.FindAsync(
            v => userVehicleIds.Contains(v.Id),
            cancellationToken);

        return vehicles.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Get vehicle by ID - VERIFIES user has access
    /// </summary>
    public async Task<VehicleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Verificar acceso del usuario
        if (!await _segmentationService.UserHasAccessToVehicleAsync(id, cancellationToken))
        {
            return null; // Usuario no tiene acceso
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(id, cancellationToken);
        if (vehicle == null)
            return null;

        return MapToDto(vehicle);
    }

    /// <summary>
    /// Get all vehicles for admin (no user filtering)
    /// </summary>
    public async Task<IEnumerable<VehicleDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default)
    {
        var vehicles = await _vehicleRepository.GetAllAsync(cancellationToken);
        return vehicles.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Get vehicles by person
    /// </summary>
    public async Task<IEnumerable<VehicleDto>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        var vehicles = await _vehicleRepository.FindAsync(
            v => v.PersonId == personId,
            cancellationToken);

        return vehicles.Select(MapToDto).ToList();
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
    /// Get vehicles for Account Person (persona de la cuenta)
    /// </summary>
    public async Task<IEnumerable<VehicleDto>> GetAccountPersonVehiclesAsync(CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await GetByPersonAsync(accountPersonId, cancellationToken);
    }

    /// <summary>
    /// Create vehicle for Account Person (persona de la cuenta)
    /// </summary>
    public async Task<VehicleDto> CreateAccountPersonVehicleAsync(CreateVehicleDto dto, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        dto.PersonId = accountPersonId;
        return await CreateAsync(dto, cancellationToken);
    }

    /// <summary>
    /// Get vehicles for a Provider
    /// </summary>
    public async Task<IEnumerable<VehicleDto>> GetProviderVehiclesAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        return await GetByPersonAsync(providerId, cancellationToken);
    }

    /// <summary>
    /// Create vehicle for a Provider
    /// </summary>
    public async Task<VehicleDto> CreateProviderVehicleAsync(Guid providerId, CreateVehicleDto dto, CancellationToken cancellationToken = default)
    {
        dto.PersonId = providerId;
        return await CreateAsync(dto, cancellationToken);
    }

    /// <summary>
    /// Get vehicles for a Client
    /// </summary>
    public async Task<IEnumerable<VehicleDto>> GetClientVehiclesAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        return await GetByPersonAsync(clientId, cancellationToken);
    }

    /// <summary>
    /// Create vehicle for a Client
    /// </summary>
    public async Task<VehicleDto> CreateClientVehicleAsync(Guid clientId, CreateVehicleDto dto, CancellationToken cancellationToken = default)
    {
        dto.PersonId = clientId;
        return await CreateAsync(dto, cancellationToken);
    }

    public async Task<VehicleDto> CreateAsync(CreateVehicleDto dto, CancellationToken cancellationToken = default)
    {
        // Si no se especifica PersonId, usar Account Person
        if (!dto.PersonId.HasValue)
        {
            var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
            dto.PersonId = accountPersonId;
        }

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            LicensePlate = dto.LicensePlate,
            VehicleType = dto.VehicleType,
            Model = dto.Model,
            Make = dto.Make,
            Year = dto.Year,
            PersonId = dto.PersonId.Value,
            MaxCapacity = dto.MaxCapacity,
            CapacityUnit = dto.CapacityUnit,
            IsAvailable = dto.IsAvailable,
            SpecialEquipment = dto.SpecialEquipment,
            VirtualFacilityId = dto.VirtualFacilityId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(vehicle);
    }

    public async Task<VehicleDto?> UpdateAsync(UpdateVehicleDto dto, CancellationToken cancellationToken = default)
    {
        // Verificar acceso del usuario
        if (!await _segmentationService.UserHasAccessToVehicleAsync(dto.Id, cancellationToken))
        {
            return null; // Usuario no tiene acceso
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (vehicle == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(dto.LicensePlate))
            vehicle.LicensePlate = dto.LicensePlate;
        if (!string.IsNullOrEmpty(dto.VehicleType))
            vehicle.VehicleType = dto.VehicleType;
        if (dto.Model != null)
            vehicle.Model = dto.Model;
        if (dto.Make != null)
            vehicle.Make = dto.Make;
        if (dto.Year.HasValue)
            vehicle.Year = dto.Year;
        if (dto.MaxCapacity.HasValue)
            vehicle.MaxCapacity = dto.MaxCapacity;
        if (dto.CapacityUnit != null)
            vehicle.CapacityUnit = dto.CapacityUnit;
        if (dto.IsAvailable.HasValue)
            vehicle.IsAvailable = dto.IsAvailable.Value;
        if (dto.SpecialEquipment != null)
            vehicle.SpecialEquipment = dto.SpecialEquipment;
        if (dto.VirtualFacilityId.HasValue)
            vehicle.VirtualFacilityId = dto.VirtualFacilityId;
        if (dto.IsActive.HasValue)
            vehicle.IsActive = dto.IsActive.Value;

        vehicle.UpdatedAt = DateTime.UtcNow;

        await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(vehicle);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Verificar acceso del usuario
        if (!await _segmentationService.UserHasAccessToVehicleAsync(id, cancellationToken))
        {
            return false; // Usuario no tiene acceso
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(id, cancellationToken);
        if (vehicle == null)
            return false;

        await _vehicleRepository.DeleteAsync(vehicle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private VehicleDto MapToDto(Vehicle vehicle)
    {
        return new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            VehicleType = vehicle.VehicleType,
            Model = vehicle.Model,
            Make = vehicle.Make,
            Year = vehicle.Year,
            PersonId = vehicle.PersonId,
            PersonName = vehicle.Person?.Name,
            MaxCapacity = vehicle.MaxCapacity,
            CapacityUnit = vehicle.CapacityUnit,
            IsAvailable = vehicle.IsAvailable,
            SpecialEquipment = vehicle.SpecialEquipment,
            VirtualFacilityId = vehicle.VirtualFacilityId,
            VirtualFacilityName = vehicle.VirtualFacility?.Name,
            CreatedAt = vehicle.CreatedAt,
            UpdatedAt = vehicle.UpdatedAt,
            IsActive = vehicle.IsActive
        };
    }
}

