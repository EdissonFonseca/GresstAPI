using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Service requests between persons/companies
/// </summary>
public class Request : BaseEntity
{
    public string RequestNumber { get; set; } = string.Empty;
    public RequestStatus Status { get; set; }
    
    // Requester (who needs the service)
    public Guid RequesterId { get; set; }
    public virtual Person Requester { get; set; } = null!;
    
    // Provider (who will provide the service)
    public Guid? ProviderId { get; set; }
    public virtual Person? Provider { get; set; }
    
    // Service Details
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ServicesRequested { get; set; } = string.Empty; // JSON array: ["Collection", "Transport", "Disposal"]
    
    // Dates
    public DateTime RequestedDate { get; set; }
    public DateTime? RequiredByDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    // Location
    public string? PickupAddress { get; set; }
    public string? DeliveryAddress { get; set; }
    
    // Financial
    public decimal? EstimatedCost { get; set; }
    public decimal? AgreedCost { get; set; }
    public decimal? ActualCost { get; set; }
    
    // Navigation properties
    public virtual ICollection<RequestItem> Items { get; set; } = new List<RequestItem>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

/// <summary>
/// Items in a request (waste types/quantities requested)
/// </summary>
public class RequestItem : BaseEntity
{
    public Guid RequestId { get; set; }
    public virtual Request Request { get; set; } = null!;
    
    public Guid WasteTypeId { get; set; }
    public virtual WasteType WasteType { get; set; } = null!;
    
    public decimal EstimatedQuantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    
    public string? Description { get; set; }
}

