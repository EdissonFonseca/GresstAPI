namespace Gresst.Domain.Enums;

public enum WasteItemStatus
{
    Declared,       // Exists from the request
    Scheduled,      // Programmed for collection - Only Collection
    Collected,      // Collected from generator - Only Collection
    InTransit,      // In transit to plant - Only Collection
    Received,       // Received at plant
    Processing,     // Being processed in plant
    Completed,      // Process completed
    Cancelled,      // Cancelled in plant or request
    Rejected        // Rejected before reception
}

