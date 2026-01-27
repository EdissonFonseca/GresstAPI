using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ClientDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClientDto>> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<ClientDto> CreateAsync(CreateClientDto dto, CancellationToken cancellationToken = default);
    Task<ClientDto?> UpdateAsync(UpdateClientDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

