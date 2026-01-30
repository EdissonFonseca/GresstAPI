using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerDto>> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<CustomerDto?> UpdateAsync(UpdateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
