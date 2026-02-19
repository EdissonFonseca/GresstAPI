using Gresst.Domain.Common;
using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Service requests between persons/companies
/// </summary>
public class Request : BaseEntity
{
    public string RequestNumber { get; set; } = string.Empty;
    
    public RequestStatus Status { get; set; } = RequestStatus.Draft;
    public string? RequesterId { get; set; }
    public string? ProviderId { get; set; }
    public HandoverType HandoverType { get; set; }
      // Collection | ReceptionHouse
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<WasteItem> Items { get; set; } = new List<WasteItem>();
   public ICollection<RequestEvent> Events { get; set; } = new List<RequestEvent>();

    public void Apply(RequestEvent requestEvent)
    {
        Status = requestEvent.ToStatus;
        Events.Add(requestEvent);
    }}
