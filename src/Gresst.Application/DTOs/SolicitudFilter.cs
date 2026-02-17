namespace Gresst.Application.DTOs;

/// <summary>
/// Filter for querying solicitudes (requests) by current state and context.
/// Request = client–provider link; repository returns only request data. Order (planning/execution) is applied in the application layer when needed.
/// </summary>
public class SolicitudFilter
{
    /// <summary>
    /// Specific solicitud IDs to return (e.g. for GetById).
    /// </summary>
    public IReadOnlyList<long>? SolicitudIds { get; set; }

    /// <summary>
    /// Multitenant: logged-in user's account person id. When set, only rows with Solicitud.IdPersona equal to this value are returned.
    /// </summary>
    public string? AccountPersonId { get; set; }

    /// <summary>
    /// Optional: filter by IdSolicitante (requester).
    /// </summary>
    public IReadOnlyList<string>? SolicitanteIds { get; set; }

    /// <summary>
    /// Optional: filter by IdProveedor (provider).
    /// </summary>
    public IReadOnlyList<string>? ProveedorIds { get; set; }

    /// <summary>
    /// Optional: filter by IdTransportador (assigned driver/transporter).
    /// </summary>
    public IReadOnlyList<string>? TransportadorIds { get; set; }

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
