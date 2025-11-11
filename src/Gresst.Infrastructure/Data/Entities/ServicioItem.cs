using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdItem", "IdRelacion", "IdServicio")]
[Table("Servicio_Item")]
public partial class ServicioItem
{
    [Key]
    public long IdItem { get; set; }

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    public long IdServicio { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdItem")]
    [InverseProperty("ServicioItemIdItemNavigations")]
    public virtual Servicio IdItemNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("ServicioItems")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;

    [ForeignKey("IdServicio")]
    [InverseProperty("ServicioItemIdServicioNavigations")]
    public virtual Servicio IdServicioNavigation { get; set; } = null!;
}
