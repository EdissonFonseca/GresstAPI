namespace Gresst.Application.DTOs;

/// <summary>
/// Result of registering generated waste: created Residuo id and inventory (Saldo) data.
/// </summary>
public class RegisteredWasteResultDto
{
    public long IdResiduo { get; set; }
    public long IdDeposito { get; set; }
    public decimal? Cantidad { get; set; }
    public decimal? Peso { get; set; }
    public decimal? Volumen { get; set; }
    public DateTime FechaIngreso { get; set; }
}
