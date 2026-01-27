using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly IRepository<Balance> _balanceRepository;
    private readonly IRepository<Waste> _wasteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BalanceService(
        IRepository<Balance> balanceRepository,
        IRepository<Waste> wasteRepository,
        IUnitOfWork unitOfWork)
    {
        _balanceRepository = balanceRepository;
        _wasteRepository = wasteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BalanceDto>> GetInventoryAsync(InventoryQueryDto query, CancellationToken cancellationToken = default)
    {
        var balances = await _balanceRepository.GetAllAsync(cancellationToken);
        
        // Apply filters
        if (query.PersonId.HasValue)
            balances = balances.Where(b => b.PersonId == query.PersonId);
        if (query.FacilityId.HasValue)
            balances = balances.Where(b => b.FacilityId == query.FacilityId);
        if (query.LocationId.HasValue)
            balances = balances.Where(b => b.LocationId == query.LocationId);
        if (query.WasteClassId.HasValue)
            balances = balances.Where(b => b.WasteClassId == query.WasteClassId);

        return balances.Select(b => new BalanceDto
        {
            Id = b.Id,
            PersonId = b.PersonId,
            FacilityId = b.FacilityId,
            LocationId = b.LocationId,
            WasteClassId = b.WasteClassId,
            Quantity = b.Quantity,
            Unit = b.Unit.ToString(),
            QuantityGenerated = b.QuantityGenerated,
            QuantityInTransit = b.QuantityInTransit,
            QuantityStored = b.QuantityStored,
            QuantityDisposed = b.QuantityDisposed,
            QuantityTreated = b.QuantityTreated,
            LastUpdated = b.LastUpdated
        }).ToList();
    }

    public async Task<BalanceDto?> GetBalanceAsync(Guid? personId, Guid? facilityId, Guid? locationId, Guid wasteTypeId, CancellationToken cancellationToken = default)
    {
        var balances = await _balanceRepository.FindAsync(
            b => b.WasteClassId == wasteTypeId &&
                 (!personId.HasValue || b.PersonId == personId) &&
                 (!facilityId.HasValue || b.FacilityId == facilityId) &&
                 (!locationId.HasValue || b.LocationId == locationId),
            cancellationToken);

        var balance = balances.FirstOrDefault();
        if (balance == null) return null;

        return new BalanceDto
        {
            Id = balance.Id,
            PersonId = balance.PersonId,
            FacilityId = balance.FacilityId,
            LocationId = balance.LocationId,
            WasteClassId = balance.WasteClassId,
            Quantity = balance.Quantity,
            Unit = balance.Unit.ToString(),
            QuantityGenerated = balance.QuantityGenerated,
            QuantityInTransit = balance.QuantityInTransit,
            QuantityStored = balance.QuantityStored,
            QuantityDisposed = balance.QuantityDisposed,
            QuantityTreated = balance.QuantityTreated,
            LastUpdated = balance.LastUpdated
        };
    }

    public async Task UpdateBalanceAsync(Guid wasteId, string operation, decimal quantity, CancellationToken cancellationToken = default)
    {
        var waste = await _wasteRepository.GetByIdAsync(wasteId.ToString(), cancellationToken);
        if (waste == null) return;

        // Find or create balance
        var balances = await _balanceRepository.FindAsync(
            b => b.WasteClassId == waste.WasteClassId &&
                 b.PersonId == waste.CurrentOwnerId &&
                 b.FacilityId == waste.CurrentFacilityId &&
                 b.LocationId == waste.CurrentLocationId,
            cancellationToken);

        var balance = balances.FirstOrDefault();
        if (balance == null)
        {
            balance = new Balance
            {
                PersonId = waste.CurrentOwnerId,
                FacilityId = waste.CurrentFacilityId,
                LocationId = waste.CurrentLocationId,
                WasteClassId = waste.WasteClassId,
                Unit = waste.Unit,
                Quantity = 0,
                QuantityGenerated = 0,
                QuantityInTransit = 0,
                QuantityStored = 0,
                QuantityDisposed = 0,
                QuantityTreated = 0,
                LastUpdated = DateTime.UtcNow
            };
            await _balanceRepository.AddAsync(balance, cancellationToken);
        }

        // Update quantities based on operation
        switch (operation.ToLower())
        {
            case "generate":
                balance.QuantityGenerated += quantity;
                balance.Quantity += quantity;
                break;
            case "collect":
                balance.QuantityInTransit += quantity;
                break;
            case "transport":
                balance.QuantityInTransit += quantity;
                break;
            case "store":
            case "storetemporary":
            case "storepermanent":
                balance.QuantityStored += quantity;
                balance.QuantityInTransit -= quantity;
                break;
            case "dispose":
                balance.QuantityDisposed += quantity;
                balance.Quantity -= quantity;
                break;
            case "treat":
            case "transform":
                balance.QuantityTreated += quantity;
                break;
        }

        balance.LastUpdated = DateTime.UtcNow;
        await _balanceRepository.UpdateAsync(balance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

