using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Record of waste management operations (Generate, Collect, Transport, Dispose, etc.)
/// Provides complete traceability
/// </summary>
public class Management : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public ManagementType Type { get; set; }
    public DateTime ExecutedAt { get; set; }
    
    // Waste being managed
    public string WasteId { get; set; } = string.Empty;
    public virtual Waste Waste { get; set; } = null!;
    
    // Quantity managed
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    
    // Who performed the operation
    public string ExecutedById { get; set; } = string.Empty;
    public virtual Person ExecutedBy { get; set; } = null!;
    
    // Origin and Destination
    public string? OriginLocationId { get; set; }
    public virtual Location? OriginLocation { get; set; }
    
    public string? OriginFacilityId { get; set; }
    public virtual Facility? OriginFacility { get; set; }
    
    public string? DestinationLocationId { get; set; }
    public virtual Location? DestinationLocation { get; set; }
    
    public string? DestinationFacilityId { get; set; }
    public virtual Facility? DestinationFacility { get; set; }
    
    // Related entities
    public string? OrderId { get; set; }
    public virtual Order? Order { get; set; }
    
    public string? VehicleId { get; set; }
    public virtual Vehicle? Vehicle { get; set; }
    
    public string? TreatmentId { get; set; }
    public virtual Treatment? Treatment { get; set; }
    
    // Service performed
    public string ServiceId { get; set; } = string.Empty;
    public virtual Service Service { get; set; } = null!;
    
    // Documentation
    public string? Notes { get; set; }
    public string? AttachmentUrls { get; set; } // JSON array of file URLs
    
    // Certificate generation
    public string? CertificateId { get; set; }
    public virtual Certificate? Certificate { get; set; }
}

