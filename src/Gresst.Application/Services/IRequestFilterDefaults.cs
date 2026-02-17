namespace Gresst.Application.Services;

/// <summary>
/// Provides persistence-specific filter values for request (solicitud) queries.
/// Implementation lives in Infrastructure so that DB codes (IdEstado, IdServicio) are not in Application.
/// </summary>
public interface IRequestFilterDefaults
{
    /// <summary>
    /// Estado codes considered "active" for transport/reception flows (used in filter).
    /// </summary>
    IReadOnlyList<string> GetActiveEstadosForTransport();

    /// <summary>
    /// Service ID used for mobile transport (used in filter when querying by service).
    /// </summary>
    long GetTransportServiceId();
}
