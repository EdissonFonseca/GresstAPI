using MediatR;

/// <summary>
/// Domain event that can be published via MediatR to trigger side effects (e.g. creating Operation records).
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}