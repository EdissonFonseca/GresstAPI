using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IOrderService
{
    Task<OrderDto> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetByClientAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetScheduledAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateAsync(dynamic dto, CancellationToken cancellationToken = default);
    Task<OrderDto> ScheduleAsync(Guid id, DateTime scheduledDate, Guid? vehicleId, Guid? routeId, CancellationToken cancellationToken = default);
    Task<OrderDto> StartAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderDto> CompleteAsync(Guid id, decimal? actualCost, CancellationToken cancellationToken = default);
    Task CancelAsync(Guid id, string? reason, CancellationToken cancellationToken = default);
}

