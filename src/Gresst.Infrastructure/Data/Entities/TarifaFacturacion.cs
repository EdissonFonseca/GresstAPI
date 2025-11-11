using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCuenta", "IdTarifa", "FechaInicio")]
[Table("Tarifa_Facturacion")]
public partial class TarifaFacturacion
{
    [Key]
    public long IdCuenta { get; set; }

    [Key]
    public long IdTarifa { get; set; }

    [Key]
    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public bool Facturado { get; set; }

    [StringLength(50)]
    public string? NumeroFactura { get; set; }

    [StringLength(50)]
    public string? ReferenciaFactura { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaEmisionFactura { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorFactura { get; set; }

    [StringLength(255)]
    public string? NotasFactura { get; set; }

    public bool Pagado { get; set; }

    [StringLength(100)]
    public string? ReferenciaPago { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaPago { get; set; }

    public string? NotasPago { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorPago { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Descuento { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cargo { get; set; }

    public int? TransaccionesCobradas { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Impuesto { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Retencion { get; set; }

    public long? IdMoneda { get; set; }

    [ForeignKey("IdCuenta")]
    [InverseProperty("TarifaFacturacions")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    [ForeignKey("IdMoneda")]
    [InverseProperty("TarifaFacturacions")]
    public virtual Monedum? IdMonedaNavigation { get; set; }

    [ForeignKey("IdTarifa")]
    [InverseProperty("TarifaFacturacions")]
    public virtual Tarifa IdTarifaNavigation { get; set; } = null!;
}
