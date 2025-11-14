using Gresst.Domain.Common;

namespace Gresst.Domain.Entities;

/// <summary>
/// Licenses for persons/facilities to operate
/// </summary>
public class License : BaseEntity
{
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty; // Collection, Transport, Disposal, Treatment, Storage
    
    // Holder
    public Guid? PersonId { get; set; }
    public virtual Person? Person { get; set; }
    
    public Guid? FacilityId { get; set; }
    public virtual Facility? Facility { get; set; }
    
    // Issuing Authority
    public string IssuingAuthority { get; set; } = string.Empty;
    
    // Validity
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public new bool IsActive { get; set; } = true;
    
    // Scope
    public string? AuthorizedWasteClasses { get; set; } // JSON array of waste class IDs
    public string? Restrictions { get; set; }
    
    // Documentation
    public string? DocumentUrl { get; set; }
}

