using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Material service with user-level data segmentation
/// Automatically filters materials based on current user's assignments
/// </summary>
public class MaterialService : IMaterialService
{
    private readonly IRepository<Material> _materialRepository;
    private readonly IRepository<PersonMaterial> _personMaterialRepository;
    private readonly IRepository<FacilityMaterial> _facilityMaterialRepository;
    private readonly IRepository<Facility> _facilityRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IDataSegmentationService _segmentationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    // Role codes
    private const string CLIENT_ROLE_CODE = "CL";
    private const string PROVIDER_ROLE_CODE = "PR";

    public MaterialService(
        IRepository<Material> materialRepository,
        IRepository<PersonMaterial> personMaterialRepository,
        IRepository<FacilityMaterial> facilityMaterialRepository,
        IRepository<Facility> facilityRepository,
        IAccountRepository accountRepository,
        IDataSegmentationService segmentationService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _materialRepository = materialRepository;
        _personMaterialRepository = personMaterialRepository;
        _facilityMaterialRepository = facilityMaterialRepository;
        _facilityRepository = facilityRepository;
        _accountRepository = accountRepository;
        _segmentationService = segmentationService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Get all materials - FILTERED by current user assignments
    /// </summary>
    public async Task<IEnumerable<MaterialDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Obtener IDs de materiales asignados al usuario actual
        var userMaterialIds = await _segmentationService.GetUserMaterialIdsAsync(cancellationToken);
        
        // Si es admin, devolver todos
        if (await _segmentationService.CurrentUserIsAdminAsync(cancellationToken))
        {
            var allMaterials = await _materialRepository.GetAllAsync(cancellationToken);
            return allMaterials.Select(MapToDto).ToList();
        }

        // Usuario normal: solo sus materiales asignados
        var materials = await _materialRepository.FindAsync(
            m => userMaterialIds.Contains(m.Id),
            cancellationToken);

        return materials.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Get material by ID - VERIFIES user has access
    /// </summary>
    public async Task<MaterialDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // Verificar acceso del usuario
        if (!await _segmentationService.UserHasAccessToMaterialAsync(id, cancellationToken))
        {
            return null; // Usuario no tiene acceso
        }

        var material = await _materialRepository.GetByIdAsync(id, cancellationToken);
        if (material == null)
            return null;

        return MapToDto(material);
    }

    /// <summary>
    /// Get all materials for admin (no user filtering)
    /// </summary>
    public async Task<IEnumerable<MaterialDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default)
    {
        var materials = await _materialRepository.GetAllAsync(cancellationToken);
        return materials.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<MaterialDto>> GetByWasteClassAsync(string wasteTypeId, CancellationToken cancellationToken = default)
    {
        var userMaterialIds = await _segmentationService.GetUserMaterialIdsAsync(cancellationToken);
        
        var materials = await _materialRepository.FindAsync(
            m => m.WasteClassId == wasteTypeId && userMaterialIds.Contains(m.Id), 
            cancellationToken);
        
        return materials.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<MaterialDto>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        var userMaterialIds = await _segmentationService.GetUserMaterialIdsAsync(cancellationToken);
        
        var materials = await _materialRepository.FindAsync(
            m => m.Category == category && userMaterialIds.Contains(m.Id), 
            cancellationToken);
        
        return materials.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<MaterialDto>> GetBySynonimsAsync(string name, CancellationToken cancellationToken = default)
    {
        var userMaterialIds = await _segmentationService.GetUserMaterialIdsAsync(cancellationToken);

        var materials = await _materialRepository.FindAsync(
            m => m.Category == name && userMaterialIds.Contains(m.Id),
            cancellationToken);

        return materials.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Get Account Person ID (persona de la cuenta)
    /// </summary>
    private async Task<string> GetAccountPersonIdAsync(CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PersonId))
            throw new InvalidOperationException("Account or Account Person not found");
        return account.PersonId;
    }

    /// <summary>
    /// Get materials for Account Person (persona de la cuenta)
    /// </summary>
    public async Task<IEnumerable<MaterialDto>> GetAccountPersonMaterialsAsync(CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        var personMaterials = await _personMaterialRepository.FindAsync(
            pm => pm.PersonId == accountPersonId && pm.IsActive,
            cancellationToken);
        
        var materialIds = personMaterials.Select(pm => pm.MaterialId).Distinct().ToList();
        var materials = await _materialRepository.FindAsync(
            m => materialIds.Contains(m.Id),
            cancellationToken);
        
        return materials.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Create material for Account Person (persona de la cuenta)
    /// </summary>
    public async Task<MaterialDto> CreateAccountPersonMaterialAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await CreateMaterialForPersonAsync(accountPersonId, dto, cancellationToken);
    }

    /// <summary>
    /// Get materials for a Provider
    /// </summary>
    public async Task<IEnumerable<MaterialDto>> GetProviderMaterialsAsync(string providerId, CancellationToken cancellationToken = default)
    {
        var personMaterials = await _personMaterialRepository.FindAsync(
            pm => pm.PersonId == providerId && pm.IsActive,
            cancellationToken);
        
        var materialIds = personMaterials.Select(pm => pm.MaterialId).Distinct().ToList();
        var materials = await _materialRepository.FindAsync(
            m => materialIds.Contains(m.Id),
            cancellationToken);
        
        return materials.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Create material for a Provider
    /// </summary>
    public async Task<MaterialDto> CreateProviderMaterialAsync(string providerId, CreateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        return await CreateMaterialForPersonAsync(providerId, dto, cancellationToken);
    }

    /// <summary>
    /// Get materials for a Customer
    /// </summary>
    public async Task<IEnumerable<MaterialDto>> GetCustomerMaterialsAsync(string customerId, CancellationToken cancellationToken = default)
    {
        var personMaterials = await _personMaterialRepository.FindAsync(
            pm => pm.PersonId == customerId && pm.IsActive,
            cancellationToken);
        
        var materialIds = personMaterials.Select(pm => pm.MaterialId).Distinct().ToList();
        var materials = await _materialRepository.FindAsync(
            m => materialIds.Contains(m.Id),
            cancellationToken);
        
        return materials.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Create material for a Customer
    /// </summary>
    public async Task<MaterialDto> CreateCustomerMaterialAsync(string customerId, CreateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        return await CreateMaterialForPersonAsync(customerId, dto, cancellationToken);
    }

    /// <summary>
    /// Get materials for a Facility
    /// </summary>
    public async Task<IEnumerable<MaterialDto>> GetFacilityMaterialsAsync(string facilityId, CancellationToken cancellationToken = default)
    {
        var facilityMaterials = await _facilityMaterialRepository.FindAsync(
            fm => fm.FacilityId == facilityId && fm.IsActive,
            cancellationToken);
        
        var materialIds = facilityMaterials.Select(fm => fm.MaterialId).Distinct().ToList();
        var materials = await _materialRepository.FindAsync(
            m => materialIds.Contains(m.Id),
            cancellationToken);
        
        return materials.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Create material for a Facility
    /// </summary>
    public async Task<MaterialDto> CreateFacilityMaterialAsync(string facilityId, CreateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        // Verify facility exists and get its owner
        var facility = await _facilityRepository.GetByIdAsync(facilityId.ToString(), cancellationToken);
        if (facility == null)
            throw new InvalidOperationException("Facility not found");

        // Create the material
        var material = new Material
        {
            Id = string.Empty,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            WasteClassId = dto.WasteClassId,
            IsRecyclable = dto.IsRecyclable,
            IsHazardous = dto.IsHazardous,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _materialRepository.AddAsync(material, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Create FacilityMaterial relationship (material.Id is now set by DbContext)
        var facilityMaterial = new FacilityMaterial
        {
            Id = string.Empty,
            PersonId = facility.PersonId,
            FacilityId = facilityId,
            MaterialId = material.Id,
            ServicePrice = dto.ServicePrice,
            PurchasePrice = dto.PurchasePrice,
            Weight = dto.Weight,
            Volume = dto.Volume,
            EmissionCompensationFactor = dto.EmissionCompensationFactor,
            IsHandled = true,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _facilityMaterialRepository.AddAsync(facilityMaterial, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(material);
    }

    /// <summary>
    /// Helper method to create material for a person
    /// </summary>
    private async Task<MaterialDto> CreateMaterialForPersonAsync(string personId, CreateMaterialDto dto, CancellationToken cancellationToken)
    {
        // Create the material
        var material = new Material
        {
            Id = string.Empty,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            WasteClassId = dto.WasteClassId,
            IsRecyclable = dto.IsRecyclable,
            IsHazardous = dto.IsHazardous,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _materialRepository.AddAsync(material, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Create PersonMaterial relationship (material.Id is now set by DbContext)
        var personMaterial = new PersonMaterial
        {
            Id = string.Empty,
            PersonId = personId,
            MaterialId = material.Id,
            Name = dto.Name,
            ServicePrice = dto.ServicePrice,
            PurchasePrice = dto.PurchasePrice,
            Weight = dto.Weight,
            Volume = dto.Volume,
            EmissionCompensationFactor = dto.EmissionCompensationFactor,
            Reference = dto.Reference,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _personMaterialRepository.AddAsync(personMaterial, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(material);
    }

    public async Task<MaterialDto> CreateAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        // By default, create for Account Person
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await CreateMaterialForPersonAsync(accountPersonId, dto, cancellationToken);
    }

    public async Task<MaterialDto?> UpdateAsync(UpdateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        // Verificar acceso del usuario
        if (!await _segmentationService.UserHasAccessToMaterialAsync(dto.Id, cancellationToken))
        {
            return null; // Usuario no tiene acceso
        }

        var material = await _materialRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (material == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(dto.Name))
            material.Name = dto.Name;
        if (dto.Description != null)
            material.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.WasteClassId))
            material.WasteClassId = dto.WasteClassId;
        if (dto.IsRecyclable.HasValue)
            material.IsRecyclable = dto.IsRecyclable.Value;
        if (dto.IsHazardous.HasValue)
            material.IsHazardous = dto.IsHazardous.Value;
        if (dto.Category != null)
            material.Category = dto.Category;
        if (dto.IsActive.HasValue)
            material.IsActive = dto.IsActive.Value;

        material.UpdatedAt = DateTime.UtcNow;

        await _materialRepository.UpdateAsync(material, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(material);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        // Verificar acceso del usuario
        if (!await _segmentationService.UserHasAccessToMaterialAsync(id, cancellationToken))
        {
            return false; // Usuario no tiene acceso
        }

        var material = await _materialRepository.GetByIdAsync(id, cancellationToken);
        if (material == null)
            return false;

        await _materialRepository.DeleteAsync(material, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private MaterialDto MapToDto(Material material)
    {
        // Mapeo básico desde Domain Material al DTO
        // Los campos adicionales (ServicePrice, PurchasePrice, etc.) se pueden agregar
        // al Domain Material en el futuro o mapear directamente desde el Repository
        return new MaterialDto
        {
            Id = material.Id,
            Code = material.Code,
            Name = material.Name,
            Description = material.Description,
            WasteClassId = material.WasteClassId,
            WasteClassName = material.WasteClass?.Name,
            IsRecyclable = material.IsRecyclable,
            IsHazardous = material.IsHazardous,
            Category = material.Category,
            IsActive = material.IsActive,
            CreatedAt = material.CreatedAt,
            UpdatedAt = material.UpdatedAt
            // Nota: Campos adicionales (ServicePrice, PurchasePrice, Weight, Volume, etc.)
            // están en la BD pero no en el Domain Material actual.
            // Se pueden agregar al Domain o crear un método específico en el Repository
            // que devuelva el DTO directamente desde la BD.
        };
    }
}

