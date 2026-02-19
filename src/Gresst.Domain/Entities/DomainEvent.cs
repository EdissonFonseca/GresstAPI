public abstract class DomainEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EntityId { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? TriggeredById { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}