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
    public string? RequestId { get; set; }
    public virtual Request? Request { get; set; }
    
    // Service to be performed
    public string ServiceId { get; set; } = string.Empty;
    public virtual Service Service { get; set; } = null!;
    
    // Service Provider
    public string ProviderId { get; set; } = string.Empty;
    public virtual Person Provider { get; set; } = null!;
    
    // Customer
    public string CustomerId { get; set; } = string.Empty;
    public virtual Person Customer { get; set; } = null!;
    
    // Resources
    public string? VehicleId { get; set; }
    public virtual Vehicle? Vehicle { get; set; }
    
    public string? FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    
    public string? RouteId { get; set; }
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
    public string OrderId { get; set; } = string.Empty;
    public virtual Order Order { get; set; } = null!;
    
    public string? WasteId { get; set; }
    public virtual Waste? Waste { get; set; }
    
    public string? WasteClassId { get; set; }
    public virtual WasteClass? WasteClass { get; set; }
    
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    
    public string? Notes { get; set; }
}

