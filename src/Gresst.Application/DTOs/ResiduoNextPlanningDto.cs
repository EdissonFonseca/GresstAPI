namespace Gresst.Application.DTOs;

/// <summary>
/// Next planned order (Orden) for a residuo, from OrdenResiduo + Orden.
/// Order = internal planning/execution for processes (treatment, disposal, transformation). Answers "When? Who is responsible?"
/// </summary>
public class ResiduoNextPlanningDto
{
    public long IdResiduo { get; set; }
    public long IdOrden { get; set; }
    public long? NumeroOrden { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public long? IdTratamiento { get; set; }
    public string? IdResponsable { get; set; }
}
