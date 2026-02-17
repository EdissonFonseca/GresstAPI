using Gresst.Application.Services;
using Gresst.Infrastructure.WasteManagement;

namespace Gresst.Infrastructure.Services;

/// <summary>
/// Provides persistence-specific filter values for request queries.
/// Keeps DB codes (IdEstado, IdServicio) out of Application.
/// </summary>
public class RequestFilterDefaults : IRequestFilterDefaults
{
    public IReadOnlyList<string> GetActiveEstadosForTransport() =>
        RequestDbStateCodes.EstadosActivosTransporte;

    public long GetTransportServiceId() =>
        RequestDbStateCodes.IdServicioTransporteMovil;
}
