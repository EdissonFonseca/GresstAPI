using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdItem", "IdRelacion", "IdClasificacion")]
[Table("Clasificacion_Item")]
public partial class ClasificacionItem
{
    [Key]
    [StringLength(10)]
    public string IdItem { get; set; } = null!;

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    [StringLength(10)]
    public string IdClasificacion { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdClasificacion")]
    [InverseProperty("ClasificacionItemIdClasificacionNavigations")]
    public virtual Clasificacion IdClasificacionNavigation { get; set; } = null!;

    [ForeignKey("IdItem")]
    [InverseProperty("ClasificacionItemIdItemNavigations")]
    public virtual Clasificacion IdItemNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("ClasificacionItems")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;
}
