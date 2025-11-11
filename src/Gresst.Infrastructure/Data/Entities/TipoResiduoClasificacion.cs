using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdTipoResiduo", "IdClasificacion")]
[Table("TipoResiduo_Clasificacion")]
public partial class TipoResiduoClasificacion
{
    [Key]
    public int IdTipoResiduo { get; set; }

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
    [InverseProperty("TipoResiduoClasificacions")]
    public virtual Clasificacion IdClasificacionNavigation { get; set; } = null!;

    [ForeignKey("IdTipoResiduo")]
    [InverseProperty("TipoResiduoClasificacions")]
    public virtual TipoResiduo IdTipoResiduoNavigation { get; set; } = null!;
}
