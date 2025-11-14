using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Work orders for waste management operations
/// </summary>
public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; }
    
    // Scheduling
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    // Related Request
    public Guid? RequestId { get; set; }
    public virtual Request? Request { get; set; }
    
    // Service to be performed
    public Guid ServiceId { get; set; }
    public virtual Service Service { get; set; } = null!;
    
    // Service Provider
    public Guid ProviderId { get; set; }
    public virtual Person Provider { get; set; } = null!;
    
    // Client
    public Guid ClientId { get; set; }
    public virtual Person Client { get; set; } = null!;
    
    // Resources
    public Guid? VehicleId { get; set; }
    public virtual Vehicle? Vehicle { get; set; }
    
    public Guid? FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    
    public Guid? RouteId { get; set; }
    public virtual Route? Route { get; set; }
    
    // Details
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    
    // Financial
    public decimal? EstimatedCost { get; set; }
    public decimal? ActualCost { get; set; }
    
    // Navigation properties
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public virtual ICollection<Management> Managements { get; set; } = new List<Management>();
}

/// <summary>
/// Items in an order (specific wastes)
/// </summary>
public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    
    public Guid? WasteId { get; set; }
    public virtual Waste? Waste { get; set; }
    
    public Guid? WasteClassId { get; set; }
    public virtual WasteClass? WasteClass { get; set; }
    
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    
    public string? Notes { get; set; }
}

