using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Tarifa")]
public partial class Tarifa
{
    [Key]
    public long IdTarifa { get; set; }

    [StringLength(1)]
    public string IdRol { get; set; } = null!;

    public long IdMoneda { get; set; }

    public DateOnly FechaInicio { get; set; }

    [StringLength(50)]
    public string Descripcion { get; set; } = null!;

    [StringLength(50)]
    public string Referencia { get; set; } = null!;

    public DateOnly? FechaFin { get; set; }

    [StringLength(1)]
    public string Modalidad { get; set; } = null!;

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorServicio { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorTransaccion { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? MaximoTransacciones { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? ValorTransaccionAdicional { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdMoneda")]
    [InverseProperty("Tarifas")]
    public virtual Monedum IdMonedaNavigation { get; set; } = null!;

    [InverseProperty("IdTarifaNavigation")]
    public virtual ICollection<TarifaCuentum> TarifaCuenta { get; set; } = new List<TarifaCuentum>();

    [InverseProperty("IdTarifaNavigation")]
    public virtual ICollection<TarifaFacturacion> TarifaFacturacions { get; set; } = new List<TarifaFacturacion>();

    [InverseProperty("IdTarifaNavigation")]
    public virtual ICollection<TarifaOpcion> TarifaOpcions { get; set; } = new List<TarifaOpcion>();
}
