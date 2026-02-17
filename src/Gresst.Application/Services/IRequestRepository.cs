using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Repository for Request (Solicitud) operations.
/// Request = client–provider link (generators vs managers, or managers vs other managers).
/// Returns only request state; Order (Orden) data is applied in the application layer for planning/execution views.
/// See docs/requests-and-orders.md.
/// </summary>
public interface IRequestRepository
{
    /// <summary>
    /// Gets solicitudes (requests) with details by filter (state, dates, person, service).
    /// No join with Orden/OrdenPlaneacion — planning is added by the application layer when needed.
    /// </summary>
    Task<IEnumerable<SolicitudWithDetailsDto>> GetSolicitudesAsync(
        SolicitudFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order planning (OrdenPlaneacion + Orden) for the given (IdSolicitud, IdDepositoOrigen) keys.
    /// Order = internal planning/execution (when, who is responsible). Used to enrich requests for transport/collections views.
    /// </summary>
    Task<IReadOnlyList<OrdenPlanningDto>> GetOrdenPlanningForSolicitudesAsync(
        IReadOnlyList<(long IdSolicitud, long IdDepositoOrigen)> keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next planned Orden for the given residuo (from OrdenResiduo + Orden).
    /// Order = internal planning/execution for processes (treatment, disposal, transformation).
    /// Returns the order with the nearest FechaInicio that is not yet finished (FechaFin null or in the future).
    /// </summary>
    Task<ResiduoNextPlanningDto?> GetNextOrdenPlanningForResiduoAsync(
        long idResiduo,
        CancellationToken cancellationToken = default);
}

