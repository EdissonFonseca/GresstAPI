using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

public class WasteService : IWasteService
{
    private readonly IRepository<Waste> _wasteRepository;
    private readonly IRepository<WasteClass> _wasteTypeRepository;
    private readonly IRepository<Person> _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WasteService(
        IRepository<Waste> wasteRepository,
        IRepository<WasteClass> wasteTypeRepository,
        IRepository<Person> personRepository,
        IUnitOfWork unitOfWork)
    {
        _wasteRepository = wasteRepository;
        _wasteTypeRepository = wasteTypeRepository;
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WasteDto> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(id, cancellationToken);
        if (waste == null)
            throw new KeyNotFoundException($"Waste with ID {id} not found");

        return await MapToDto(waste, cancellationToken);
    }

    public async Task<IEnumerable<WasteDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var wastes = await _wasteRepository.GetAllAsync(cancellationToken);
        var dtos = new List<WasteDto>();
        
        foreach (var waste in wastes)
        {
            dtos.Add(await MapToDto(waste, cancellationToken));
        }
        
        return dtos;
    }

    public async Task<IEnumerable<WasteDto>> GetByGeneratorAsync(string generatorId, CancellationToken cancellationToken = default)
    {
        var wastes = await _wasteRepository.FindAsync(w => w.GeneratorId == generatorId, cancellationToken);
        var dtos = new List<WasteDto>();
        
        foreach (var waste in wastes)
        {
            dtos.Add(await MapToDto(waste, cancellationToken));
        }
        
        return dtos;
    }

    public async Task<IEnumerable<WasteDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<WasteStatus>(status, out var wasteStatus))
            throw new ArgumentException($"Invalid status: {status}");

        var wastes = await _wasteRepository.FindAsync(w => w.Status == wasteStatus, cancellationToken);
        var dtos = new List<WasteDto>();
        
        foreach (var waste in wastes)
        {
            dtos.Add(await MapToDto(waste, cancellationToken));
        }
        
        return dtos;
    }

    public async Task<IEnumerable<WasteDto>> GetWasteBankAsync(CancellationToken cancellationToken = default)
    {
        var wastes = await _wasteRepository.FindAsync(w => w.IsAvailableInBank, cancellationToken);
        var dtos = new List<WasteDto>();
        
        foreach (var waste in wastes)
        {
            dtos.Add(await MapToDto(waste, cancellationToken));
        }
        
        return dtos;
    }

    public async Task<WasteDto> CreateAsync(CreateWasteDto dto, CancellationToken cancellationToken = default)
    {
        var waste = new Waste
        {
            Code = dto.Code,
            Description = dto.Description,
            WasteClassId = dto.WasteClassId,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            Status = WasteStatus.Generated,
            GeneratorId = dto.GeneratorId,
            GeneratedAt = DateTime.UtcNow,
            CurrentOwnerId = dto.GeneratorId,
            CurrentLocationId = dto.CurrentLocationId,
            CurrentFacilityId = dto.CurrentFacilityId,
            PackagingId = dto.PackagingId,
            IsHazardous = dto.IsHazardous,
            BatchNumber = dto.BatchNumber,
            ContainerNumber = dto.ContainerNumber
        };

        await _wasteRepository.AddAsync(waste, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapToDto(waste, cancellationToken);
    }

    public async Task<WasteDto> UpdateAsync(UpdateWasteDto dto, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (waste == null)
            throw new KeyNotFoundException($"Waste with ID {dto.Id} not found");

        if (dto.Description != null)
            waste.Description = dto.Description;
        if (dto.Quantity.HasValue)
            waste.Quantity = dto.Quantity.Value;
        if (dto.CurrentLocationId != null)
            waste.CurrentLocationId = dto.CurrentLocationId;
        if (dto.CurrentFacilityId != null)
            waste.CurrentFacilityId = dto.CurrentFacilityId;
        if (dto.IsAvailableInBank.HasValue)
            waste.IsAvailableInBank = dto.IsAvailableInBank.Value;
        if (dto.BankDescription != null)
            waste.BankDescription = dto.BankDescription;
        if (dto.BankPrice.HasValue)
            waste.BankPrice = dto.BankPrice;

        await _wasteRepository.UpdateAsync(waste, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapToDto(waste, cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(id, cancellationToken);
        if (waste == null)
            throw new KeyNotFoundException($"Waste with ID {id} not found");

        await _wasteRepository.DeleteAsync(waste, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task PublishToWasteBankAsync(string wasteId, string description, decimal? price, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(wasteId.ToString(), cancellationToken);
        if (waste == null)
            throw new KeyNotFoundException($"Waste with ID {wasteId} not found");

        waste.IsAvailableInBank = true;
        waste.BankDescription = description;
        waste.BankPrice = price;

        await _wasteRepository.UpdateAsync(waste, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFromWasteBankAsync(string wasteId, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(wasteId.ToString(), cancellationToken);
        if (waste == null)
            throw new KeyNotFoundException($"Waste with ID {wasteId} not found");

        waste.IsAvailableInBank = false;
        waste.BankDescription = null;
        waste.BankPrice = null;

        await _wasteRepository.UpdateAsync(waste, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<WasteDto> MapToDto(Waste waste, CancellationToken cancellationToken = default)
    {
        // Temporary: Get related entities with null safety
        WasteClass? wasteType = null;
        Person? generator = null;
        
        try
        {
            wasteType = await _wasteTypeRepository.GetByIdAsync(waste.WasteClassId.ToString(), cancellationToken);
            generator = await _personRepository.GetByIdAsync(waste.GeneratorId.ToString(), cancellationToken);
        }
        catch
        {
            // If repositories not fully implemented, continue without related data
        }

        return new WasteDto
        {
            Id = waste.Id,
            Code = waste.Code,
            Description = waste.Description,
            WasteClassId = waste.WasteClassId,
            WasteClassName = wasteType?.Name ?? $"Type-{waste.WasteClassId}",
            Quantity = waste.Quantity,
            Unit = waste.Unit.ToString(),
            Status = waste.Status.ToString(),
            GeneratorId = waste.GeneratorId,
            GeneratorName = generator?.Name ?? $"Generator-{waste.GeneratorId}",
            GeneratedAt = waste.GeneratedAt,
            CurrentLocationId = waste.CurrentLocationId,
            CurrentFacilityId = waste.CurrentFacilityId,
            IsHazardous = waste.IsHazardous,
            IsAvailableInBank = waste.IsAvailableInBank,
            BankPrice = waste.BankPrice
        };
    }
}

