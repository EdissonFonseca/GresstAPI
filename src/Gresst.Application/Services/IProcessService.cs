using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing processes, subprocesses, and tasks
/// </summary>
public interface IProcessService
{
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
}

