using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Repository interface for Request-specific operations
/// </summary>
public interface IRequestRepository
{
    /// <summary>
    /// Gets mobile transport waste data implementing the fnResiduosTransporteMovil logic
    /// </summary>
    /// <param name="personId">Person ID (Domain Guid)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of mobile transport waste data</returns>
    Task<IEnumerable<MobileTransportWasteDto>> GetMobileTransportWasteAsync(
        Guid personId, 
        CancellationToken cancellationToken = default);
}

