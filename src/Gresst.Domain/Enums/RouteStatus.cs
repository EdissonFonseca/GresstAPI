public enum RouteStatus
{
    Draft,          // Creada pero no planificada
    Planned,        // Stops definidos y lista para ejecución
    Approved,       // Validada por supervisor / sistema
    Dispatched,     // Vehículo asignado y salida autorizada
    InProgress,     // Ruta en ejecución
    Paused,         // Detenida temporalmente
    Completed,      // Finalizada correctamente
    CompletedWithIssues, // Finalizada con incidentes
    Cancelled,      // Cancelada antes de finalizar
    Failed          // No pudo completarse
}