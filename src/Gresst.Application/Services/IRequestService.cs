using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IRequestService
{
    Task<RequestDto> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDto>> GetByRequesterAsync(string requesterId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDto>> GetByProviderAsync(string providerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RequestDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<RequestDto> CreateAsync(CreateRequestDto dto, CancellationToken cancellationToken = default);
    Task<RequestDto> UpdateAsync(dynamic dto, CancellationToken cancellationToken = default);
    Task<RequestDto> ApproveAsync(string id, decimal? agreedCost, CancellationToken cancellationToken = default);
    Task<RequestDto> RejectAsync(string id, string reason, CancellationToken cancellationToken = default);
    Task CancelAsync(string id, CancellationToken cancellationToken = default);
    
    // Mobile transport waste operations
    Task<IEnumerable<MobileTransportWasteDto>> GetMobileTransportWasteAsync(string personId, CancellationToken cancellationToken = default);
}

