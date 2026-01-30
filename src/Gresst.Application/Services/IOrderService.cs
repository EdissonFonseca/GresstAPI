using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IOrderService
{
    Task<OrderDto> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetByProviderAsync(string providerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetByClientAsync(string clientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetScheduledAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateAsync(dynamic dto, CancellationToken cancellationToken = default);
    Task<OrderDto> ScheduleAsync(string id, DateTime scheduledDate, string? vehicleId, string? routeId, CancellationToken cancellationToken = default);
    Task<OrderDto> StartAsync(string id, CancellationToken cancellationToken = default);
    Task<OrderDto> CompleteAsync(string id, decimal? actualCost, CancellationToken cancellationToken = default);
    Task CancelAsync(string id, string? reason, CancellationToken cancellationToken = default);
}

