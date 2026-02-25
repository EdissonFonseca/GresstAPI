public enum WasteStatus
{
    Draft,                // Registrado pero no confirmado
    Generated,            // Generado oficialmente
    Available,            // Disponible para recolección o transferencia
    Collected,            // Recogido en ruta
    InTransit,            // En transporte
    Received,             // Recibido en instalación
    InProcessing,         // En tratamiento activo
    Processed,            // Tratado
    StoredTemporarily,    // En almacenamiento temporal
    ReadyForDisposal,     // Aprobado para disposición final
    Disposed,             // Disposición final ejecutada
    Contained,            // Confinado permanentemente
    Rejected,             // Rechazado por instalación
    Returned,             // Devuelto al generador
    Lost,                 // Pérdida física
    Incident,             // Asociado a incidente
    Cancelled             // Registro anulado
}