using Gresst.Domain.Enums;

public class RequestEvent : DomainEvent
{
    public RequestStatus FromStatus { get; set; }
    public RequestStatus ToStatus { get; set; }
}