using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IRequestService
{
    Task<RequestDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDto>> GetByRequesterAsync(Guid requesterId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDto>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<RequestDto> CreateAsync(CreateRequestDto dto, CancellationToken cancellationToken = default);
    Task<RequestDto> UpdateAsync(dynamic dto, CancellationToken cancellationToken = default);
    Task<RequestDto> ApproveAsync(Guid id, decimal? agreedCost, CancellationToken cancellationToken = default);
    Task<RequestDto> RejectAsync(Guid id, string reason, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid id, CancellationToken cancellationToken = default);
}

