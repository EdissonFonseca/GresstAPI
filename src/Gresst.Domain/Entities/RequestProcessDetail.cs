using Gresst.Domain.Enums;

namespace Gresst.Domain.Entities;

/// <summary>
/// Process view of a request (solicitud) detail line. Used by process services to apply stage/phase rules and build transport/collection views.
/// Returned by repository; no DTOs from repository to service.
/// </summary>
public class RequestProcessDetail
{
    public long IdSolicitud { get; set; }
    public long? NumeroSolicitud { get; set; }
    public DateTime? FechaSolicitud { get; set; }
    public int? Ocurrencia { get; set; }
    public string? Recurrencia { get; set; }
    public string IdEstado { get; set; } = string.Empty;
    public bool? MultiplesGeneradores { get; set; }

    public int Item { get; set; }
    public string? IdSolicitante { get; set; }
    public long? IdDepositoOrigen { get; set; }
    public string? IdProveedor { get; set; }
    public long? IdDepositoDestino { get; set; }
    public string? IdVehiculo { get; set; }
    public long? IdResiduo { get; set; }
    public long? IdMaterial { get; set; }
    public string? Descripcion { get; set; }
    public long? IdTratamiento { get; set; }
    public DateTime? FechaInicioDetalle { get; set; }
    public decimal? CantidadSolicitud { get; set; }
    public decimal? PesoSolicitud { get; set; }
    public decimal? VolumenSolicitud { get; set; }
    public decimal? Cantidad { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Volumen { get; set; }
    public long? IdEmbalaje { get; set; }
    public decimal? PrecioCompra { get; set; }
    public decimal? PrecioServicio { get; set; }
    public string? IdEtapa { get; set; }
    public string? IdFase { get; set; }

    public RequestFlowStage? Stage { get; set; }
    public RequestFlowPhase? Phase { get; set; }

    public string? Soporte { get; set; }
    public string? Notas { get; set; }
    public bool? Procesado { get; set; }
    public string? IdCausa { get; set; }
    public string? IdGrupo { get; set; }
    public string? Titulo { get; set; }

    public string? Material { get; set; }
    public string? Medicion { get; set; }
    public decimal? PesoUnitario { get; set; }
    public decimal? PrecioUnitario { get; set; }
    public decimal? PrecioServicioUnitario { get; set; }
    public string? Solicitante { get; set; }
    public string? DireccionOrigen { get; set; }
    public double? LatitudOrigen { get; set; }
    public double? LongitudOrigen { get; set; }
    public string? DireccionDestino { get; set; }
    public double? LatitudDestino { get; set; }
    public double? LongitudDestino { get; set; }
    public string? Proveedor { get; set; }
    public string? DepositoOrigen { get; set; }
    public string? DepositoDestino { get; set; }
    public string? Tratamiento { get; set; }
    public string? Embalaje { get; set; }
    public string? EmbalajeSolicitud { get; set; }
}
