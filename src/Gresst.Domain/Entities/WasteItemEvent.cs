using Gresst.Domain.Enums;

public class WasteItemEvent : DomainEvent
{
    public WasteItemStatus FromStatus { get; set; }
    public WasteItemStatus ToStatus { get; set; }
    public OperationType Operation { get; set; }
    public HandoverType HandoverType { get; set; }
    public string? ProcedureId { get; set; }
    public string? OrderId { get; set; }
    public string? FromFacilityId { get; set; }
    public string? ToFacilityId { get; set; }
    public string? FromPartyId { get; set; }
    public string? ToPartyId { get; set; }
}