using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdSolicitud", "IdInsumo")]
[Table("Solicitud_Insumo")]
public partial class SolicitudInsumo
{
    [Key]
    public long IdSolicitud { get; set; }

    [Key]
    public long IdInsumo { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    public int? IdUnidad { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdInsumo")]
    [InverseProperty("SolicitudInsumos")]
    public virtual Insumo IdInsumoNavigation { get; set; } = null!;

    [ForeignKey("IdSolicitud")]
    [InverseProperty("SolicitudInsumos")]
    public virtual Solicitud IdSolicitudNavigation { get; set; } = null!;

    [ForeignKey("IdUnidad")]
    [InverseProperty("SolicitudInsumos")]
    public virtual Unidad? IdUnidadNavigation { get; set; }

    [ForeignKey("IdUsuarioCreacion")]
    [InverseProperty("SolicitudInsumoIdUsuarioCreacionNavigations")]
    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioUltimaModificacion")]
    [InverseProperty("SolicitudInsumoIdUsuarioUltimaModificacionNavigations")]
    public virtual Usuario? IdUsuarioUltimaModificacionNavigation { get; set; }
}
