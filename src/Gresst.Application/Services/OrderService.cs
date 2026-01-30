using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing Orders (Órdenes de servicio)
/// </summary>
public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public OrderService(
        IRepository<Order> orderRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<OrderDto> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return null!;

        return MapToDto(order);
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<OrderDto>> GetByProviderAsync(string providerId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.FindAsync(
            o => o.ProviderId == providerId,
            cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<OrderDto>> GetByClientAsync(string clientId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.FindAsync(
            o => o.ClientId == clientId,
            cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<OrderDto>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
        {
            return new List<OrderDto>();
        }

        var orders = await _orderRepository.FindAsync(
            o => o.Status == orderStatus,
            cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<OrderDto>> GetScheduledAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.FindAsync(
            o => o.ScheduledDate.HasValue 
                && o.ScheduledDate.Value >= startDate 
                && o.ScheduledDate.Value <= endDate,
            cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            Type = dto.Type,
            Status = OrderStatus.Pending,
            ProviderId = dto.ProviderId,
            ClientId = dto.ClientId,
            RequestId = dto.RequestId,
            ServiceId = string.Empty, // TODO: Get ServiceId from Request or add to CreateOrderDto
            ScheduledDate = dto.ScheduledDate,
            Description = dto.Description,
            EstimatedCost = dto.EstimatedCost,
            VehicleId = dto.VehicleId,
            FacilityId = dto.FacilityId,
            RouteId = dto.RouteId
        };

        // Add items
        foreach (var itemDto in dto.Items)
        {
            order.Items.Add(new OrderItem
            {
                WasteId = itemDto.WasteId,
                WasteClassId = itemDto.WasteClassId,
                Quantity = itemDto.Quantity,
                Unit = itemDto.Unit,
                Notes = itemDto.Notes
            });
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> UpdateAsync(dynamic dto, CancellationToken cancellationToken = default)
    {
        // Extract ID from dynamic object
        string id;
        try
        {
            id = (string)dto.Id;
        }
        catch
        {
            throw new ArgumentException("Order ID is required", nameof(dto));
        }

        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return null!;

        // Update fields if provided
        try
        {
            if (dto.Type != null)
                order.Type = (OrderType)dto.Type;
            
            if (dto.ProviderId != null)
                order.ProviderId = (string)dto.ProviderId;
            
            if (dto.ClientId != null)
                order.ClientId = (string)dto.ClientId;
            
            if (dto.RequestId != null)
                order.RequestId = (string?)dto.RequestId;
            
            if (dto.ScheduledDate != null)
                order.ScheduledDate = (DateTime?)dto.ScheduledDate;
            
            if (dto.Description != null)
                order.Description = (string?)dto.Description;
            
            if (dto.EstimatedCost != null)
                order.EstimatedCost = (decimal?)dto.EstimatedCost;
            
            if (dto.VehicleId != null)
                order.VehicleId = (string?)dto.VehicleId;
            
            if (dto.FacilityId != null)
                order.FacilityId = (string?)dto.FacilityId;
            
            if (dto.RouteId != null)
                order.RouteId = (string?)dto.RouteId;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Error updating order: {ex.Message}", nameof(dto));
        }

        // Update items if provided
        if (dto.Items != null)
        {
            // Remove existing items
            order.Items.Clear();
            
            // Add new items
            foreach (var itemDto in dto.Items)
            {
                order.Items.Add(new OrderItem
                {
                    WasteId = itemDto.WasteId,
                    WasteClassId = itemDto.WasteClassId,
                    Quantity = itemDto.Quantity,
                    Unit = itemDto.Unit,
                    Notes = itemDto.Notes
                });
            }
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> ScheduleAsync(string id, DateTime scheduledDate, string? vehicleId, string? routeId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return null!;

        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.OnHold)
            throw new InvalidOperationException("Only pending or on-hold orders can be scheduled");

        order.Status = OrderStatus.Scheduled;
        order.ScheduledDate = scheduledDate;
        
        if (!string.IsNullOrEmpty(vehicleId))
            order.VehicleId = vehicleId;
        
        if (!string.IsNullOrEmpty(routeId))
            order.RouteId = routeId;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> StartAsync(string id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return null!;

        if (order.Status != OrderStatus.Scheduled && order.Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only scheduled or pending orders can be started");

        order.Status = OrderStatus.InProgress;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<OrderDto> CompleteAsync(string id, decimal? actualCost, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return null!;

        if (order.Status != OrderStatus.InProgress)
            throw new InvalidOperationException("Only in-progress orders can be completed");

        order.Status = OrderStatus.Completed;
        order.CompletedDate = DateTime.UtcNow;
        
        if (actualCost.HasValue)
            order.ActualCost = actualCost;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task CancelAsync(string id, string? reason, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return;

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel a completed or already cancelled order");

        order.Status = OrderStatus.Cancelled;
        
        if (!string.IsNullOrEmpty(reason))
        {
            order.Description = string.IsNullOrEmpty(order.Description) 
                ? $"Cancelled: {reason}" 
                : $"{order.Description}\n\nCancelled: {reason}";
        }

        await _orderRepository.UpdateAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // Helper methods
    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Type = order.Type.ToString(),
            Status = order.Status.ToString(),
            ProviderId = order.ProviderId,
            ProviderName = order.Provider?.Name ?? string.Empty,
            ClientId = order.ClientId,
            ClientName = order.Client?.Name ?? string.Empty,
            ScheduledDate = order.ScheduledDate,
            CompletedDate = order.CompletedDate,
            Description = order.Description,
            EstimatedCost = order.EstimatedCost,
            ActualCost = order.ActualCost,
            Items = order.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                WasteId = item.WasteId,
                WasteClassId = item.WasteClassId,
                WasteClassName = item.WasteClass?.Name ?? string.Empty,
                Quantity = item.Quantity,
                Unit = item.Unit.ToString()
            }).ToList()
        };
    }

    private string GenerateOrderNumber()
    {
        // Simple implementation - in production, use a proper numbering service
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
    }
}

