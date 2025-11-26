using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing processes, subprocesses, and tasks
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// Maps transport waste data to process hierarchy
    /// </summary>
    /// <param name="transportData">Transport waste data to map</param>
    /// <returns>List of processes with subprocesses and tasks</returns>
    Task<IEnumerable<ProcessDto>> MapTransportDataToProcessesAsync(
        IEnumerable<MobileTransportWasteDto> transportData,
        CancellationToken cancellationToken = default);
}

