using Gresst.Domain.Enums;

public class OrderEvent : DomainEvent
{
    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }
    public string? WasteItemEventId { get; set; }   // evento que generó en el residuo
}