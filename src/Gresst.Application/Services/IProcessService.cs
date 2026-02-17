using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing processes, subprocesses, and tasks
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// Gets mobile transport waste data for the account (solicitudes + planning). Used by collections and transport endpoints.
    /// </summary>
    /// <param name="accountPersonId">Account person ID (IdPersona or IdTransportador on Solicitud)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of transport waste rows with order/planning data</returns>
    Task<IEnumerable<MobileTransportWasteDto>> GetMobileTransportWasteForAccountAsync(
        string accountPersonId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets solicitudes (with details) pending collection for the account. Uses GetSolicitudesAsync + stage rule.
    /// </summary>
    Task<IEnumerable<SolicitudWithDetailsDto>> GetPendientesRecoleccionAsync(
        string accountPersonId,
        long? idServicio = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets items pending collection with planning data (Orden, FechaInicio, IdResponsable).
    /// Optional filters: date (planned date), driverId (assigned responsible/conductor).
    /// </summary>
    Task<IEnumerable<MobileTransportWasteDto>> GetPendientesRecoleccionWithPlanningAsync(
        string accountPersonId,
        DateTime? date = null,
        string? driverId = null,
        long? idServicio = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets solicitudes (with details) pending reception for the account.
    /// </summary>
    Task<IEnumerable<SolicitudWithDetailsDto>> GetPendientesRecepcionAsync(
        string accountPersonId,
        long? idServicio = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets solicitudes (with details) pending treatment for the account.
    /// </summary>
    Task<IEnumerable<SolicitudWithDetailsDto>> GetPendientesTratamientoAsync(
        string accountPersonId,
        long? idServicio = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets collection tasks for the mobile app (Recolecciones): transport tasks assigned to the current driver, optionally for a given date.
    /// Equivalent to the legacy GetTransaction / fnResiduosPendientesRecoleccion: only rows where IdResponsable = personId.
    /// </summary>
    /// <param name="personId">Driver person ID (assigned responsible)</param>
    /// <param name="date">Optional date to filter by process/order date; when null, returns all pending for the driver</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of processes (orders) with subprocesses (collection points) and tasks (request details)</returns>
    Task<IEnumerable<ProcessDto>> GetCollectionsForDriverAsync(
        string personId,
        DateTime? date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Maps transport waste data to process hierarchy
    /// </summary>
    /// <param name="transportData">Transport waste data to map</param>
    /// <returns>List of processes with subprocesses and tasks</returns>
    Task<IEnumerable<ProcessDto>> MapTransportDataToProcessesAsync(
        IEnumerable<MobileTransportWasteDto> transportData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next planned activity (Orden) for a residuo, if any.
    /// Answers "Is the next operation (transformation, disposal, treatment) already planned? For when?"
    /// Planning for processes is in Orden (linked via OrdenResiduo).
    /// </summary>
    Task<ResiduoNextPlanningDto?> GetNextPlannedActivityForResiduoAsync(
        long idResiduo,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets solicitud details that are pending (collection, reception, or treatment) and have no
    /// planned next activity (no OrdenPlaneacion for the corresponding IdSolicitud + IdDepositoOrigen).
    /// Answers "Residuos that do not have a subsequent activity planned" for the request/solicitud flow.
    /// </summary>
    Task<IEnumerable<SolicitudWithDetailsDto>> GetPendientesSinActividadPlaneadaAsync(
        string accountPersonId,
        long? idServicio = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        CancellationToken cancellationToken = default);
}

