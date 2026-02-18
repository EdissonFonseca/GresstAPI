namespace Gresst.Domain.Enums;

public enum RequestItemStatus
{
    Pending,
    Scheduled,      // solo Collection
    Collected,      // solo Collection
    InTransit,      // solo Collection
    Received,       // punto de convergencia
    Processing,     // siendo procesado en planta
    Completed,      // process Completed
    Cancelled,
    Rejected        // rechazado en planta
}

