using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IProviderService
{
    Task<IEnumerable<ProviderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProviderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProviderDto>> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<ProviderDto> CreateAsync(CreateProviderDto dto, CancellationToken cancellationToken = default);
    Task<ProviderDto?> UpdateAsync(UpdateProviderDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

