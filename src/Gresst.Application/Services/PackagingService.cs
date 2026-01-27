using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Packaging service for managing packaging types
/// </summary>
public class PackagingService : IPackagingService
{
    private readonly IRepository<Packaging> _packagingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public PackagingService(
        IRepository<Packaging> packagingRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _packagingRepository = packagingRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<PackagingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var packagings = await _packagingRepository.GetAllAsync(cancellationToken);
        return packagings.Select(MapToDto).ToList();
    }

    public async Task<PackagingDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var packaging = await _packagingRepository.GetByIdAsync(id, cancellationToken);
        if (packaging == null)
            return null;

        return MapToDto(packaging);
    }

    public async Task<IEnumerable<PackagingDto>> GetByTypeAsync(string packagingType, CancellationToken cancellationToken = default)
    {
        var packagings = await _packagingRepository.FindAsync(
            p => p.PackagingType.Equals(packagingType, StringComparison.OrdinalIgnoreCase),
            cancellationToken);
        
        return packagings.Select(MapToDto).ToList();
    }

    public async Task<PackagingDto> CreateAsync(CreatePackagingDto dto, CancellationToken cancellationToken = default)
    {
        var packaging = new Packaging
        {
            Id = string.Empty,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            PackagingType = dto.PackagingType,
            Capacity = dto.Capacity,
            CapacityUnit = dto.CapacityUnit,
            IsReusable = dto.IsReusable,
            Material = dto.Material,
            UNPackagingCode = dto.UNPackagingCode,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _packagingRepository.AddAsync(packaging, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(packaging);
    }

    public async Task<PackagingDto?> UpdateAsync(UpdatePackagingDto dto, CancellationToken cancellationToken = default)
    {
        var packaging = await _packagingRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (packaging == null)
            return null;

        // Update only provided fields (PATCH-like)
        if (!string.IsNullOrEmpty(dto.Name))
            packaging.Name = dto.Name;
        if (dto.Description != null)
            packaging.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.PackagingType))
            packaging.PackagingType = dto.PackagingType;
        if (dto.Capacity.HasValue)
            packaging.Capacity = dto.Capacity;
        if (dto.CapacityUnit != null)
            packaging.CapacityUnit = dto.CapacityUnit;
        if (dto.IsReusable.HasValue)
            packaging.IsReusable = dto.IsReusable.Value;
        if (dto.Material != null)
            packaging.Material = dto.Material;
        if (dto.UNPackagingCode != null)
            packaging.UNPackagingCode = dto.UNPackagingCode;
        if (dto.IsActive.HasValue)
            packaging.IsActive = dto.IsActive.Value;

        packaging.UpdatedAt = DateTime.UtcNow;

        await _packagingRepository.UpdateAsync(packaging, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(packaging);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var packaging = await _packagingRepository.GetByIdAsync(id, cancellationToken);
        if (packaging == null)
            return false;

        await _packagingRepository.DeleteAsync(packaging, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private PackagingDto MapToDto(Packaging packaging)
    {
        return new PackagingDto
        {
            Id = packaging.Id,
            Code = packaging.Code,
            Name = packaging.Name,
            Description = packaging.Description,
            PackagingType = packaging.PackagingType,
            Capacity = packaging.Capacity,
            CapacityUnit = packaging.CapacityUnit,
            IsReusable = packaging.IsReusable,
            Material = packaging.Material,
            UNPackagingCode = packaging.UNPackagingCode,
            IsActive = packaging.IsActive,
            CreatedAt = packaging.CreatedAt,
            UpdatedAt = packaging.UpdatedAt
        };
    }
}

