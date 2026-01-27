using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IWasteService
{
    Task<WasteDto> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WasteDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WasteDto>> GetByGeneratorAsync(Guid generatorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WasteDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<WasteDto>> GetWasteBankAsync(CancellationToken cancellationToken = default);
    Task<WasteDto> CreateAsync(CreateWasteDto dto, CancellationToken cancellationToken = default);
    Task<WasteDto> UpdateAsync(UpdateWasteDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    
    // Waste Bank operations
    Task PublishToWasteBankAsync(Guid wasteId, string description, decimal? price, CancellationToken cancellationToken = default);
    Task RemoveFromWasteBankAsync(Guid wasteId, CancellationToken cancellationToken = default);
}

