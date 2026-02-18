using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Maps persistence Solicitud (+ SolicitudDetalle for state rule) to domain Request.
/// State rule for IdEstado "A": if any detail has Recibido=1, IdResiduo not null, Etapa not 'X'/'F', Fase not 'F' → Active (InProgress); else Finished (Completed).
/// </summary>
public static class SolicitudToRequestMapper
{
    /// <summary>
    /// Determines domain RequestStatus from Solicitud.IdEstado and its details.
    /// When estado is "A": if any detail with (Recibido=1, IdResiduo!=null, IdEtapa not 'X' and not 'F', IdFase != 'F') then Active (InProgress); else Completed.
    /// </summary>
    public static RequestStatus ToRequestStatus(string? idEstado, IReadOnlyList<SolicitudDetalle> details)
    {
        if (string.IsNullOrEmpty(idEstado))
            return RequestStatus.Draft;

        var estado = idEstado.ToUpperInvariant();

        if (estado == "A") // Aprobada
        {
            bool hasActiveDetail = details.Any(d =>
                d.Recibido == true
                && d.IdResiduo != null
                && d.IdEtapa != null && d.IdEtapa.ToUpperInvariant() != "X" && d.IdEtapa.ToUpperInvariant() != "F"
                && d.IdFase != null && d.IdFase.ToUpperInvariant() != "F");
            return hasActiveDetail ? RequestStatus.InProgress : RequestStatus.Completed;
        }

        return estado switch
        {
            "M" => RequestStatus.Submitted,  // Pendiente
            "R" => RequestStatus.Rejected,  // Rechazada
            "T" => RequestStatus.InProgress, // En proceso
            "C" => RequestStatus.Cancelled, // Cancelada
            _ => RequestStatus.Draft
        };
    }

    /// <summary>
    /// Maps Solicitud and its details to domain Request (no items, header only). Uses details only for state rule.
    /// </summary>
    public static Request ToRequest(Solicitud s, IReadOnlyList<SolicitudDetalle> details)
    {
        var status = ToRequestStatus(s.IdEstado, details);
        return new Request
        {
            Id = s.IdSolicitud.ToString(),
            RequestNumber = s.NumeroSolicitud?.ToString() ?? "",
            //Status = status,
            RequesterId = s.IdSolicitante ?? "",
            ProviderId = s.IdProveedor,
            //ServiceId = s.IdServicio.ToString(),
            Title = "",
            //Description = s.Notas,
            //ServicesRequested = "",
            RequestedDate = s.FechaInicio,
            RequiredByDate = s.FechaFin,
            //PickupAddress = null,
            //DeliveryAddress = null,
            //EstimatedCost = null,
            //AgreedCost = null,
            CreatedAt = s.FechaCreacion,
            UpdatedAt = s.FechaUltimaModificacion,
            IsActive = true
        };
    }
}
