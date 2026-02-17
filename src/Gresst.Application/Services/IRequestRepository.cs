using Gresst.Application.DTOs;
using Gresst.Domain.Entities;

namespace Gresst.Application.Services;

/// <summary>
/// Repository for Request (Solicitud) operations. Returns domain entities only (no DTOs).
/// Request = client–provider link (generators vs managers, or managers vs other managers).
/// See docs/requests-and-orders.md.
/// </summary>
public interface IRequestRepository
{
    /// <summary>
    /// Gets requests (Solicitud) by filter. Returns domain Request; state is computed from SolicitudDetalle (e.g. A + active detail → InProgress, else Completed).
    /// </summary>
    Task<IEnumerable<Request>> GetAllAsync(
        SolicitudFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets request detail lines for process views (transport/collection). Returns domain RequestProcessDetail for stage/phase rules.
    /// </summary>
    Task<IEnumerable<RequestProcessDetail>> GetRequestProcessDetailsAsync(
        SolicitudFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets order planning (OrdenPlaneacion + Orden) for the given (IdSolicitud, IdDepositoOrigen) keys.
    /// </summary>
    Task<IReadOnlyList<OrdenPlanningDto>> GetOrdenPlanningForSolicitudesAsync(
        IReadOnlyList<(long IdSolicitud, long IdDepositoOrigen)> keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next planned Orden for the given residuo (from OrdenResiduo + Orden).
    /// </summary>
    Task<ResiduoNextPlanningDto?> GetNextOrdenPlanningForResiduoAsync(
        long idResiduo,
        CancellationToken cancellationToken = default);
}

