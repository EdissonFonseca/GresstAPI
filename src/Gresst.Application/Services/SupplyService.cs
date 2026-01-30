using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Supply service for managing supplies/insumos used in logistics operations
/// </summary>
public class SupplyService : ISupplyService
{
    private readonly IRepository<Supply> _supplyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SupplyService(
        IRepository<Supply> supplyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _supplyRepository = supplyRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<SupplyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var supplies = await _supplyRepository.GetAllAsync(cancellationToken);
        return supplies.Select(MapToDto).ToList();
    }

    public async Task<SupplyDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var supply = await _supplyRepository.GetByIdAsync(id, cancellationToken);
        if (supply == null)
            return null;

        return MapToDto(supply);
    }

    public async Task<IEnumerable<SupplyDto>> GetByCategoryAsync(string categoryUnitId, CancellationToken cancellationToken = default)
    {
        var supplies = await _supplyRepository.FindAsync(
            s => s.CategoryUnitId.Equals(categoryUnitId, StringComparison.OrdinalIgnoreCase),
            cancellationToken);
        
        return supplies.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<SupplyDto>> GetPublicSuppliesAsync(CancellationToken cancellationToken = default)
    {
        var supplies = await _supplyRepository.FindAsync(
            s => s.IsPublic,
            cancellationToken);
        
        return supplies.Select(MapToDto).ToList();
    }

    public async Task<SupplyDto> CreateAsync(CreateSupplyDto dto, CancellationToken cancellationToken = default)
    {
        var supply = new Supply
        {
            Id = string.Empty,
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            CategoryUnitId = dto.CategoryUnitId,
            IsPublic = dto.IsPublic,
            ParentSupplyId = dto.ParentSupplyId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _supplyRepository.AddAsync(supply, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(supply);
    }

    public async Task<SupplyDto?> UpdateAsync(UpdateSupplyDto dto, CancellationToken cancellationToken = default)
    {
        var supply = await _supplyRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (supply == null)
            return null;

        // Update only provided fields (PATCH-like)
        if (!string.IsNullOrEmpty(dto.Code))
            supply.Code = dto.Code;
        if (!string.IsNullOrEmpty(dto.Name))
            supply.Name = dto.Name;
        if (dto.Description != null)
            supply.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.CategoryUnitId))
            supply.CategoryUnitId = dto.CategoryUnitId;
        if (dto.IsPublic.HasValue)
            supply.IsPublic = dto.IsPublic.Value;
        if (dto.ParentSupplyId != null)
        {
            if (string.IsNullOrEmpty(dto.ParentSupplyId))
                supply.ParentSupplyId = null;
            else
                supply.ParentSupplyId = dto.ParentSupplyId;
        }
        if (dto.IsActive.HasValue)
            supply.IsActive = dto.IsActive.Value;

        supply.UpdatedAt = DateTime.UtcNow;

        await _supplyRepository.UpdateAsync(supply, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(supply);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var supply = await _supplyRepository.GetByIdAsync(id, cancellationToken);
        if (supply == null)
            return false;

        await _supplyRepository.DeleteAsync(supply, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private SupplyDto MapToDto(Supply supply)
    {
        return new SupplyDto
        {
            Id = supply.Id,
            Code = supply.Code,
            Name = supply.Name,
            Description = supply.Description,
            CategoryUnitId = supply.CategoryUnitId,
            IsPublic = supply.IsPublic,
            ParentSupplyId = supply.ParentSupplyId,
            ParentSupplyName = supply.ParentSupply?.Name,
            IsActive = supply.IsActive,
            CreatedAt = supply.CreatedAt,
            UpdatedAt = supply.UpdatedAt
        };
    }
}

