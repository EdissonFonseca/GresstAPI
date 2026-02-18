using Gresst.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gresst.Application.WasteManagement.Requests.GetAll
{
    public class RequestFilter
    {
        // Identity
        public string? RequestNumber { get; set; }

        // Type
        public DeliveryType? DeliveryType { get; set; }
        public bool? IsRecurrency { get; set; }

        // Status
        public RequestStatus? Status { get; set; }
        public IEnumerable<RequestStatus>? Statuses { get; set; } // filtrar por varios estados

        // Parties
        public string? RequesterId { get; set; }
        public string? ProviderId { get; set; }
        public string? HaulerId { get; set; }

        // Facilities
        public string? SourceFacilityId { get; set; }
        public string? DestinationFacilityId { get; set; }

        // Vehicle
        public string? VehicleId { get; set; }

        // Dates
        public DateTime? RequestedDateFrom { get; set; }
        public DateTime? RequestedDateTo { get; set; }
        public DateTime? RequiredByDateFrom { get; set; }
        public DateTime? RequiredByDateTo { get; set; }
        public DateTime? ApprovedDateFrom { get; set; }
        public DateTime? ApprovedDateTo { get; set; }
        public DateTime? CompletedDateFrom { get; set; }
        public DateTime? CompletedDateTo { get; set; }

        // Pagination
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Ordering
        public RequestOrderBy OrderBy { get; set; } = RequestOrderBy.RequestedDate;
        public bool OrderDescending { get; set; } = true;
    }

    public enum RequestOrderBy
    {
        RequestedDate,
        RequiredByDate,
        RequestNumber,
        Status
    }
}
