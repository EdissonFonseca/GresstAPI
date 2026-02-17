namespace Gresst.Application.DTOs;

/// <summary>
/// Filter for querying solicitudes (requests) by current state and context.
/// Request = client–provider link; repository returns only request data. Order (planning/execution) is applied in the application layer when needed.
/// </summary>
public class SolicitudFilter
{
    /// <summary>
    /// Account or person IDs to filter by (IdPersona or IdTransportador on Solicitud).
    /// </summary>
    public IReadOnlyList<string>? PersonIds { get; set; }

    /// <summary>
    /// Service ID (e.g. 8 for mobile transport).
    /// </summary>
    public long? IdServicio { get; set; }

    /// <summary>
    /// Allowed estado codes (e.g. M, A, R, T).
    /// </summary>
    public IReadOnlyList<string>? Estados { get; set; }

    /// <summary>
    /// Optional date range on FechaInicio (solicitud or detail).
    /// </summary>
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// If true, exclude solicitudes with Recurrencia set.
    /// </summary>
    public bool ExcludeRecurring { get; set; } = true;
}
