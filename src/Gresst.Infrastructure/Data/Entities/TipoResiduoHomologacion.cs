using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdTipoResiduo", "IdCategoria", "Id")]
[Table("TipoResiduo_Homologacion")]
public partial class TipoResiduoHomologacion
{
    [Key]
    public int IdTipoResiduo { get; set; }

    [Key]
    [StringLength(20)]
    public string IdCategoria { get; set; } = null!;

    [Key]
    [StringLength(20)]
    public string Id { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("Id, IdCategoria")]
    [InverseProperty("TipoResiduoHomologacions")]
    public virtual Codificacion Codificacion { get; set; } = null!;

    [ForeignKey("IdTipoResiduo")]
    [InverseProperty("TipoResiduoHomologacions")]
    public virtual TipoResiduo IdTipoResiduoNavigation { get; set; } = null!;
}
