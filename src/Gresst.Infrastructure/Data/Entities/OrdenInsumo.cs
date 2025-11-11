using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdOrden", "IdInsumo", "FechaHora")]
[Table("Orden_Insumo")]
public partial class OrdenInsumo
{
    [Key]
    public long IdOrden { get; set; }

    [Key]
    public long IdInsumo { get; set; }

    [Key]
    [Column(TypeName = "datetime")]
    public DateTime FechaHora { get; set; }

    public Geometry? Ubicacion { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal Cantidad { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Precio { get; set; }

    public int IdUnidad { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdInsumo")]
    [InverseProperty("OrdenInsumos")]
    public virtual Insumo IdInsumoNavigation { get; set; } = null!;

    [ForeignKey("IdOrden")]
    [InverseProperty("OrdenInsumos")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [ForeignKey("IdUnidad")]
    [InverseProperty("OrdenInsumos")]
    public virtual Unidad IdUnidadNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioCreacion")]
    [InverseProperty("OrdenInsumoIdUsuarioCreacionNavigations")]
    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioUltimaModificacion")]
    [InverseProperty("OrdenInsumoIdUsuarioUltimaModificacionNavigations")]
    public virtual Usuario? IdUsuarioUltimaModificacionNavigation { get; set; }
}
