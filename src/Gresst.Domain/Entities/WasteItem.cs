using System.ComponentModel;
using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Individual waste item with complete traceability
/// </summary>
public class WasteItem : BaseEntity
{
    public string? Description { get; set; }
    
    public string? WasteTypeId { get; set; }
    public string? ParentId { get; set; }
    public string? FacilityId { get; set; }
    public string? GeneratorId { get; set; }
    public string? OwnerId { get; set; }
    public string? PackagingId { get; set; }  
    public WasteItemStatus Status { get; set; } = WasteItemStatus.Declared;
    public Measurement? EstimatedMeasurement { get; set; }
    public Measurement? ActualMeasurement { get; set; }
    public Money? EstimatedPrice { get; set; }
    public Money? ActualPrice { get; set; }
    public ICollection<WasteItemEvent> Events { get; set; } = new List<WasteItemEvent>();
    public void Apply(WasteItemEvent wasteEvent)
    {
        Status = wasteEvent.ToStatus;
        if (wasteEvent.ToPartyId != null)    OwnerId    = wasteEvent.ToPartyId;
        if (wasteEvent.ToFacilityId != null) FacilityId = wasteEvent.ToFacilityId;
        Events.Add(wasteEvent);
    }
}
