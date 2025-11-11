using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdItem", "IdRelacion", "IdTipoResiduo")]
[Table("TipoResiduo_Item")]
public partial class TipoResiduoItem
{
    [Key]
    public int IdItem { get; set; }

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    public int IdTipoResiduo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdItem")]
    [InverseProperty("TipoResiduoItemIdItemNavigations")]
    public virtual TipoResiduo IdItemNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("TipoResiduoItems")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;

    [ForeignKey("IdTipoResiduo")]
    [InverseProperty("TipoResiduoItemIdTipoResiduoNavigations")]
    public virtual TipoResiduo IdTipoResiduoNavigation { get; set; } = null!;
}
