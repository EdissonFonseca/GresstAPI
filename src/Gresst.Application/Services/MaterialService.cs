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
    private readonly IDataSegmentationService _segmentationService;
    private readonly IUnitOfWork _unitOfWork;

    public MaterialService(
        IRepository<Material> materialRepository,
        IDataSegmentationService segmentationService,
        IUnitOfWork unitOfWork)
    {
        _materialRepository = materialRepository;
        _segmentationService = segmentationService;
        _unitOfWork = unitOfWork;
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
    public async Task<MaterialDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
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

    public async Task<IEnumerable<MaterialDto>> GetByWasteTypeAsync(Guid wasteTypeId, CancellationToken cancellationToken = default)
    {
        var userMaterialIds = await _segmentationService.GetUserMaterialIdsAsync(cancellationToken);
        
        var materials = await _materialRepository.FindAsync(
            m => m.WasteTypeId == wasteTypeId && userMaterialIds.Contains(m.Id), 
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

    public async Task<MaterialDto> CreateAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default)
    {
        var material = new Material
        {
            Id = Guid.NewGuid(),
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            WasteTypeId = dto.WasteTypeId,
            IsRecyclable = dto.IsRecyclable,
            IsHazardous = dto.IsHazardous,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _materialRepository.AddAsync(material, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(material);
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
        if (dto.WasteTypeId.HasValue)
            material.WasteTypeId = dto.WasteTypeId;
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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
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
            WasteTypeId = material.WasteTypeId,
            WasteTypeName = material.WasteType?.Name,
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

