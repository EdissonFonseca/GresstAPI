using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCuenta", "IdTarifa", "FechaInicio")]
[Table("Tarifa_Cuenta")]
public partial class TarifaCuentum
{
    [Key]
    public long IdCuenta { get; set; }

    [Key]
    public long IdTarifa { get; set; }

    [Key]
    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorServicioExcepcion { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorTransaccionExcepcion { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorTransaccionAdicionalExcepcion { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCuenta")]
    [InverseProperty("TarifaCuenta")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    [ForeignKey("IdTarifa")]
    [InverseProperty("TarifaCuenta")]
    public virtual Tarifa IdTarifaNavigation { get; set; } = null!;
}
