using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Work orders for waste management operations
/// </summary>
public class Order : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public OperationType Type { get; set; }
    public OrderStatus Status { get; set; }
    
    // Scheduling
    public DateTime? ScheduledAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    
    // Related Request
    public string? RequestId { get; set; }
    
    // Service to be performed
    
    // Service Provider
    public string ProviderId { get; set; } = string.Empty;
    
    // Customer
    public string CustomerId { get; set; } = string.Empty;
    
    // Resources
    public string? VehicleId { get; set; }
    
    public string? FacilityId { get; set; }
    
    public string? RouteId { get; set; }
    
    // Details
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    
    // Financial
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    
    // Navigation properties
    public virtual ICollection<WasteItem> Items { get; set; } = new List<WasteItem>();
    public ICollection<OrderEvent> Events { get; set; } = new List<OrderEvent>();

    public void Apply(OrderEvent orderEvent)
    {
        Status = orderEvent.ToStatus;
        if (orderEvent.ToStatus == OrderStatus.Completed)
            ExecutedAt = orderEvent.OccurredAt;
        Events.Add(orderEvent);
    }
}
