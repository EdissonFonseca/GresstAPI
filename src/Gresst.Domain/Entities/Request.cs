using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Service requests between persons/companies
/// </summary>
public class Request : BaseEntity
{
    public string RequestNumber { get; set; } = string.Empty;
    
    public RequestStatus Status
    {
        get
        {
            if (Items.All(i => i.Status == RequestItemStatus.Completed))
                return RequestStatus.Completed;

            if (Items.All(i => i.Status == RequestItemStatus.Rejected))
                return RequestStatus.Rejected;

            return RequestStatus.InProgress;
        }
    }

    // Requester (who needs the service)
    public string RequesterId { get; set; } = string.Empty;
    public virtual Person Requester { get; set; } = null!;
    
    // Provider (who will provide the service)
    public string? ProviderId { get; set; }
    public virtual Person? Provider { get; set; }

    public string? HaulerId { get; set; }
    public virtual Person? Hauler { get; set; }    


    public string? SourceFacilityId { get; set; }
    public virtual Facility? SourceFacility { get; set; }
    public string? DestinationFacilityId { get; set; }
    public virtual Facility? DestinationFacility { get; set; }
    public string? VehicleId { get; set; }

    // Delivery type (collection vs reception) is needed for process rules (e.g. collection → hauler involved, reception → no hauler)
    public DeliveryType DeliveryType { get; set; }  // Collection | ReceptionHo

    // Service Details
    public string Title { get; set; } = string.Empty;
    public bool? IsRecurrency { get; set; } = false;

    // Dates
    public DateTime RequestedDate { get; set; }
    public DateTime? RequiredByDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    


    // Navigation properties
    public virtual ICollection<RequestItem> Items { get; set; } = new List<RequestItem>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

/// <summary>
/// Items in a request (waste types/quantities requested)
/// </summary>
public class RequestItem : BaseEntity
{
    public string RequestId { get; set; } = string.Empty;
    public virtual Request Request { get; set; } = null!;
    
    public string WasteClassId { get; set; } = string.Empty;
    public virtual WasteClass WasteClass { get; set; } = null!;
    
    public decimal EstimatedQuantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    
    public string? Description { get; set; }
    public RequestItemStatus Status { get; set; } = RequestItemStatus.Pending;
}

