using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdSolicitud", "Item", "IdInsumo")]
[Table("SolicitudDetalle_Insumo")]
public partial class SolicitudDetalleInsumo
{
    [Key]
    public long IdSolicitud { get; set; }

    [Key]
    public int Item { get; set; }

    [Key]
    public long IdInsumo { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal Cantidad { get; set; }

    public int IdUnidad { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdInsumo")]
    [InverseProperty("SolicitudDetalleInsumos")]
    public virtual Insumo IdInsumoNavigation { get; set; } = null!;

    [ForeignKey("IdSolicitud")]
    [InverseProperty("SolicitudDetalleInsumos")]
    public virtual Solicitud IdSolicitudNavigation { get; set; } = null!;

    [ForeignKey("IdUnidad")]
    [InverseProperty("SolicitudDetalleInsumos")]
    public virtual Unidad IdUnidadNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioCreacion")]
    [InverseProperty("SolicitudDetalleInsumos")]
    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    [ForeignKey("IdSolicitud, Item")]
    [InverseProperty("SolicitudDetalleInsumos")]
    public virtual SolicitudDetalle SolicitudDetalle { get; set; } = null!;
}
