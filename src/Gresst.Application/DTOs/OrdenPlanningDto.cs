namespace Gresst.Application.DTOs;

/// <summary>
/// Internal planning/execution data (Orden + OrdenPlaneacion) for a request + origin depot.
/// Order = when and who (driver/responsible); used to enrich request data in transport/collections views.
/// </summary>
public class OrdenPlanningDto
{
    public long IdSolicitud { get; set; }
    public long IdDepositoOrigen { get; set; }
    public long IdOrden { get; set; }
    public long? NumeroOrden { get; set; }
    public string? IdResponsable { get; set; }
    public string? IdResponsable2 { get; set; }
    public DateTime? FechaInicio { get; set; }
}
