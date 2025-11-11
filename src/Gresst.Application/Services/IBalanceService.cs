using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IBalanceService
{
    Task<IEnumerable<BalanceDto>> GetInventoryAsync(InventoryQueryDto query, CancellationToken cancellationToken = default);
    Task<BalanceDto?> GetBalanceAsync(Guid? personId, Guid? facilityId, Guid? locationId, Guid wasteTypeId, CancellationToken cancellationToken = default);
    Task UpdateBalanceAsync(Guid wasteId, string operation, decimal quantity, CancellationToken cancellationToken = default);
}

