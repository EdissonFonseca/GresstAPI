using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IBalanceService
{
    Task<IEnumerable<BalanceDto>> GetInventoryAsync(InventoryQueryDto query, CancellationToken cancellationToken = default);
    Task<BalanceDto?> GetBalanceAsync(string? personId, string? facilityId, string? locationId, string wasteTypeId, CancellationToken cancellationToken = default);
    Task UpdateBalanceAsync(string wasteId, string operation, decimal quantity, CancellationToken cancellationToken = default);
}

