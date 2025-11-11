using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdOrden", "IdResiduo", "IdInsumo")]
[Table("OrdenResiduo_Insumo")]
public partial class OrdenResiduoInsumo
{
    [Key]
    public long IdOrden { get; set; }

    [Key]
    public long IdResiduo { get; set; }

    [Key]
    public long IdInsumo { get; set; }

    public int IdUnidad { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal Cantidad { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdInsumo")]
    [InverseProperty("OrdenResiduoInsumos")]
    public virtual Insumo IdInsumoNavigation { get; set; } = null!;

    [ForeignKey("IdOrden")]
    [InverseProperty("OrdenResiduoInsumos")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [ForeignKey("IdResiduo")]
    [InverseProperty("OrdenResiduoInsumos")]
    public virtual Residuo IdResiduoNavigation { get; set; } = null!;

    [ForeignKey("IdUnidad")]
    [InverseProperty("OrdenResiduoInsumos")]
    public virtual Unidad IdUnidadNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioCreacion")]
    [InverseProperty("OrdenResiduoInsumoIdUsuarioCreacionNavigations")]
    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioUltimaModificacion")]
    [InverseProperty("OrdenResiduoInsumoIdUsuarioUltimaModificacionNavigations")]
    public virtual Usuario? IdUsuarioUltimaModificacionNavigation { get; set; }

    [ForeignKey("IdOrden, IdResiduo")]
    [InverseProperty("OrdenResiduoInsumos")]
    public virtual OrdenResiduo OrdenResiduo { get; set; } = null!;
}
