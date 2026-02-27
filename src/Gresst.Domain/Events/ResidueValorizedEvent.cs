// Este evento hoy no tiene handler implementado
// Cuando el banco exista, su dominio escuchará este evento
public record ResidueValorizedEvent(
    Guid ResidueId,
    Guid? ProductId,     // null hasta que el banco exista
    decimal Quantity,
    string Unit,
    DateTime OccurredOn
) : IDomainEvent;