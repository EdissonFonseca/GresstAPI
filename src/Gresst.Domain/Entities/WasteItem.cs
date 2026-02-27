using Gresst.Domain.Common;
using Gresst.Domain.Events;

namespace Gresst.Domain.Entities;

/// <summary>
/// Individual waste item with complete traceability
/// </summary>
public class WasteItem : BaseEntity
{
    public string WasteId { get; set; } // Id of the waste type (e.g. UN code) that this item belongs to
    public string? Description { get; set; }
    public string? ParentId { get; set; }
    public string? GeneratorId { get; set; }
    public string? HolderId { get; set; }
    public string? PackagingId { get; set; }  
    public WasteItemStatus CurrentStatus { get; set; } = WasteItemStatus.Draft;
    public string? CurrentFacilityId { get; set; }
    public string? CurrentVehicleId { get; set; }
    public decimal? CurrentWeight { get; set; }
    public decimal? CurrentVolume { get; set; }
    public decimal? CurrentCount { get; set; }
    public ICollection<WasteItemEvent> Events { get; set; } = new List<WasteItemEvent>();

    public WasteItem(string wasteId)
    {
        WasteId = wasteId;
    }
    public void Apply(WasteItemEvent wasteEvent)
    {
        CurrentStatus = wasteEvent.ToStatus;
        if (wasteEvent.ToPartyId != null)    HolderId    = wasteEvent.ToPartyId;
        if (wasteEvent.ToFacilityId != null) CurrentFacilityId = wasteEvent.ToFacilityId;
        Events.Add(wasteEvent);
    }
}
