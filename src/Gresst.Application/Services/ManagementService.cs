using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

public class ManagementService : IManagementService
{
    private readonly IRepository<Management> _managementRepository;
    private readonly IRepository<Waste> _wasteRepository;
    private readonly IRepository<Person> _personRepository;
    private readonly IRepository<WasteTransformation> _transformationRepository;
    private readonly IBalanceService _balanceService;
    private readonly IWasteService _wasteService;
    private readonly IUnitOfWork _unitOfWork;

    public ManagementService(
        IRepository<Management> managementRepository,
        IRepository<Waste> wasteRepository,
        IRepository<Person> personRepository,
        IRepository<WasteTransformation> transformationRepository,
        IBalanceService balanceService,
        IWasteService wasteService,
        IUnitOfWork unitOfWork)
    {
        _managementRepository = managementRepository;
        _wasteRepository = wasteRepository;
        _personRepository = personRepository;
        _transformationRepository = transformationRepository;
        _balanceService = balanceService;
        _wasteService = wasteService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ManagementDto> CreateManagementAsync(CreateManagementDto dto, CancellationToken cancellationToken = default)
    {
        var management = new Management
        {
            Code = $"MGT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8]}",
            Type = dto.Type,
            ExecutedAt = DateTime.UtcNow,
            WasteId = dto.WasteId,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            ExecutedById = dto.ExecutedById,
            OriginLocationId = dto.OriginLocationId,
            OriginFacilityId = dto.OriginFacilityId,
            DestinationLocationId = dto.DestinationLocationId,
            DestinationFacilityId = dto.DestinationFacilityId,
            OrderId = dto.OrderId,
            VehicleId = dto.VehicleId,
            TreatmentId = dto.TreatmentId,
            Notes = dto.Notes
        };

        await _managementRepository.AddAsync(management, cancellationToken);
        
        // Update waste status based on management type
        await UpdateWasteStatus(dto.WasteId, dto.Type, dto.DestinationLocationId, dto.DestinationFacilityId, cancellationToken);
        
        // Update balance
        await _balanceService.UpdateBalanceAsync(dto.WasteId, dto.Type.ToString(), dto.Quantity, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapToDto(management, cancellationToken);
    }

    public async Task<IEnumerable<ManagementDto>> GetWasteHistoryAsync(Guid wasteId, CancellationToken cancellationToken = default)
    {
        var managements = await _managementRepository.FindAsync(m => m.WasteId == wasteId, cancellationToken);
        var dtos = new List<ManagementDto>();
        
        foreach (var management in managements.OrderBy(m => m.ExecutedAt))
        {
            dtos.Add(await MapToDto(management, cancellationToken));
        }
        
        return dtos;
    }

    public async Task<ManagementDto> GenerateWasteAsync(CreateWasteDto wasteDto, CancellationToken cancellationToken = default)
    {
        // Create the waste first
        var waste = await _wasteService.CreateAsync(wasteDto, cancellationToken);
        
        // Create management record
        var dto = new CreateManagementDto
        {
            Type = ManagementType.Generate,
            WasteId = Guid.Parse(waste.Id),
            Quantity = wasteDto.Quantity,
            Unit = wasteDto.Unit,
            ExecutedById = wasteDto.GeneratorId,
            DestinationLocationId = wasteDto.CurrentLocationId,
            DestinationFacilityId = wasteDto.CurrentFacilityId
        };
        
        return await CreateManagementAsync(dto, cancellationToken);
    }

    public async Task<ManagementDto> CollectWasteAsync(CollectWasteDto dto, CancellationToken cancellationToken = default)
    {
        var managementDto = new CreateManagementDto
        {
            Type = ManagementType.Collect,
            WasteId = dto.WasteId,
            Quantity = dto.Quantity,
            Unit = UnitOfMeasure.Kilogram,
            ExecutedById = dto.CollectorId,
            VehicleId = dto.VehicleId,
            OriginLocationId = dto.OriginLocationId,
            Notes = dto.Notes
        };
        
        return await CreateManagementAsync(managementDto, cancellationToken);
    }

    public async Task<ManagementDto> TransportWasteAsync(TransportWasteDto dto, CancellationToken cancellationToken = default)
    {
        var managementDto = new CreateManagementDto
        {
            Type = ManagementType.Transport,
            WasteId = dto.WasteId,
            Quantity = dto.Quantity,
            Unit = UnitOfMeasure.Kilogram,
            ExecutedById = dto.TransporterId,
            VehicleId = dto.VehicleId,
            OriginFacilityId = dto.OriginFacilityId,
            DestinationFacilityId = dto.DestinationFacilityId,
            Notes = dto.Notes
        };
        
        return await CreateManagementAsync(managementDto, cancellationToken);
    }

    public async Task<ManagementDto> ReceiveWasteAsync(Guid wasteId, Guid receiverId, Guid facilityId, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(wasteId.ToString(), cancellationToken);
        if (waste == null)
            throw new KeyNotFoundException($"Waste with ID {wasteId} not found");

        var managementDto = new CreateManagementDto
        {
            Type = ManagementType.Receive,
            WasteId = wasteId,
            Quantity = waste.Quantity,
            Unit = waste.Unit,
            ExecutedById = receiverId,
            DestinationFacilityId = facilityId
        };
        
        return await CreateManagementAsync(managementDto, cancellationToken);
    }

    public async Task<ManagementDto> StoreWasteAsync(StoreWasteDto dto, CancellationToken cancellationToken = default)
    {
        var managementType = dto.IsPermanent ? ManagementType.StorePermanent : ManagementType.StoreTemporary;
        
        var managementDto = new CreateManagementDto
        {
            Type = managementType,
            WasteId = dto.WasteId,
            Quantity = dto.Quantity,
            Unit = UnitOfMeasure.Kilogram,
            ExecutedById = Guid.Empty, // Should be passed
            DestinationLocationId = dto.LocationId,
            DestinationFacilityId = dto.FacilityId,
            Notes = dto.Notes
        };
        
        return await CreateManagementAsync(managementDto, cancellationToken);
    }

    public async Task<ManagementDto> DisposeWasteAsync(DisposeWasteDto dto, CancellationToken cancellationToken = default)
    {
        var managementDto = new CreateManagementDto
        {
            Type = ManagementType.Dispose,
            WasteId = dto.WasteId,
            Quantity = dto.Quantity,
            Unit = UnitOfMeasure.Kilogram,
            ExecutedById = dto.DisposerId,
            DestinationFacilityId = dto.FacilityId,
            Notes = dto.Notes
        };
        
        return await CreateManagementAsync(managementDto, cancellationToken);
    }

    public async Task<ManagementDto> TransformWasteAsync(TransformWasteDto dto, CancellationToken cancellationToken = default)
    {
        // Create new waste for result
        var sourceWaste = await _wasteRepository.GetByIdAsync(dto.SourceWasteId.ToString(), cancellationToken);
        if (sourceWaste == null)
            throw new KeyNotFoundException($"Source waste with ID {dto.SourceWasteId} not found");

        var newWasteDto = new CreateWasteDto
        {
            Code = $"TRF-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Description = $"Transformed from {sourceWaste.Code}",
            WasteClassId = dto.ResultWasteClassId,
            Quantity = dto.ResultQuantity,
            Unit = UnitOfMeasure.Kilogram,
            GeneratorId = dto.PerformedById,
            CurrentFacilityId = dto.FacilityId
        };

        var newWaste = await _wasteService.CreateAsync(newWasteDto, cancellationToken);

        // Create transformation record
        var transformation = new WasteTransformation
        {
            TransformationNumber = $"TRF-{DateTime.UtcNow:yyyyMMddHHmmss}",
            Type = dto.Type,
            TransformationDate = DateTime.UtcNow,
            SourceWasteId = dto.SourceWasteId,
            SourceQuantity = dto.SourceQuantity,
            SourceUnit = UnitOfMeasure.Kilogram,
            ResultWasteId = Guid.Parse(newWaste.Id),
            ResultQuantity = dto.ResultQuantity,
            ResultUnit = UnitOfMeasure.Kilogram,
            TreatmentId = dto.TreatmentId,
            FacilityId = dto.FacilityId,
            PerformedById = dto.PerformedById,
            Description = dto.Description
        };

        await _transformationRepository.AddAsync(transformation, cancellationToken);

        // Create management record
        var managementDto = new CreateManagementDto
        {
            Type = ManagementType.Transform,
            WasteId = dto.SourceWasteId,
            Quantity = dto.SourceQuantity,
            Unit = UnitOfMeasure.Kilogram,
            ExecutedById = dto.PerformedById,
            DestinationFacilityId = dto.FacilityId,
            TreatmentId = dto.TreatmentId,
            Notes = dto.Description
        };
        
        return await CreateManagementAsync(managementDto, cancellationToken);
    }

    public async Task<ManagementDto> ClassifyWasteAsync(Guid wasteId, Guid wasteTypeId, Guid classifiedById, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(wasteId.ToString(), cancellationToken);
        if (waste == null)
            throw new KeyNotFoundException($"Waste with ID {wasteId} not found");

        waste.WasteClassId = wasteTypeId;
        await _wasteRepository.UpdateAsync(waste, cancellationToken);

        var managementDto = new CreateManagementDto
        {
            Type = ManagementType.Classify,
            WasteId = wasteId,
            Quantity = waste.Quantity,
            Unit = waste.Unit,
            ExecutedById = classifiedById,
            Notes = $"Classified to waste type {wasteTypeId}"
        };
        
        return await CreateManagementAsync(managementDto, cancellationToken);
    }

    public async Task<ManagementDto> SellWasteAsync(Guid wasteId, Guid buyerId, decimal price, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(wasteId.ToString(), cancellationToken);
        if (waste == null)
            throw new KeyNotFoundException($"Waste with ID {wasteId} not found");

        waste.CurrentOwnerId = buyerId;
        await _wasteRepository.UpdateAsync(waste, cancellationToken);

        var managementDto = new CreateManagementDto
        {
            Type = ManagementType.Sell,
            WasteId = wasteId,
            Quantity = waste.Quantity,
            Unit = waste.Unit,
            ExecutedById = waste.CurrentOwnerId ?? waste.GeneratorId,
            Notes = $"Sold to {buyerId} for ${price}"
        };
        
        return await CreateManagementAsync(managementDto, cancellationToken);
    }

    public async Task<ManagementDto> DeliverToThirdPartyAsync(Guid wasteId, Guid recipientId, string notes, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(wasteId.ToString(), cancellationToken);
        if (waste == null)
            throw new KeyNotFoundException($"Waste with ID {wasteId} not found");

        waste.CurrentOwnerId = recipientId;
        await _wasteRepository.UpdateAsync(waste, cancellationToken);

        var managementDto = new CreateManagementDto
        {
            Type = ManagementType.Deliver,
            WasteId = wasteId,
            Quantity = waste.Quantity,
            Unit = waste.Unit,
            ExecutedById = waste.CurrentOwnerId ?? waste.GeneratorId,
            Notes = notes
        };
        
        return await CreateManagementAsync(managementDto, cancellationToken);
    }

    private async Task UpdateWasteStatus(Guid wasteId, ManagementType type, Guid? locationId, Guid? facilityId, CancellationToken cancellationToken)
    {
        var waste = await _wasteRepository.GetByIdAsync(wasteId.ToString(), cancellationToken);
        if (waste == null) return;

        waste.Status = type switch
        {
            ManagementType.Generate => WasteStatus.Generated,
            ManagementType.Collect => WasteStatus.InTransit,
            ManagementType.Transport => WasteStatus.InTransit,
            ManagementType.Receive => WasteStatus.Stored,
            ManagementType.Store or ManagementType.StoreTemporary or ManagementType.StorePermanent => WasteStatus.Stored,
            ManagementType.Dispose => WasteStatus.Disposed,
            ManagementType.Transform or ManagementType.Treat => WasteStatus.Transformed,
            ManagementType.Deliver => WasteStatus.Delivered,
            ManagementType.Sell => WasteStatus.Sold,
            _ => waste.Status
        };

        if (locationId.HasValue)
            waste.CurrentLocationId = locationId;
        if (facilityId.HasValue)
            waste.CurrentFacilityId = facilityId;

        await _wasteRepository.UpdateAsync(waste, cancellationToken);
    }

    private async Task<ManagementDto> MapToDto(Management management, CancellationToken cancellationToken = default)
    {
        Waste? waste = null;
        Person? executedBy = null;
        
        try
        {
            waste = await _wasteRepository.GetByIdAsync(management.WasteId.ToString(), cancellationToken);
            executedBy = await _personRepository.GetByIdAsync(management.ExecutedById.ToString(), cancellationToken);
        }
        catch
        {
            // Continue without related data if repositories not fully implemented
        }

        return new ManagementDto
        {
            Id = management.Id,
            Code = management.Code,
            Type = management.Type.ToString(),
            ExecutedAt = management.ExecutedAt,
            WasteId = management.WasteId,
            WasteCode = waste?.Code ?? $"Waste-{management.WasteId}",
            Quantity = management.Quantity,
            Unit = management.Unit.ToString(),
            ExecutedById = management.ExecutedById,
            ExecutedByName = executedBy?.Name ?? $"Person-{management.ExecutedById}",
            Notes = management.Notes
        };
    }
}

