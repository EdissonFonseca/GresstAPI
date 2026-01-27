using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface ISupplyService
{
    Task<IEnumerable<SupplyDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SupplyDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplyDto>> GetByCategoryAsync(string categoryUnitId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplyDto>> GetPublicSuppliesAsync(CancellationToken cancellationToken = default);
    Task<SupplyDto> CreateAsync(CreateSupplyDto dto, CancellationToken cancellationToken = default);
    Task<SupplyDto?> UpdateAsync(UpdateSupplyDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

