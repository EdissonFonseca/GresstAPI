using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdTipoResiduo", "IdCaracteristica")]
[Table("TipoResiduo_Caracteristica")]
public partial class TipoResiduoCaracteristica
{
    [Key]
    public int IdTipoResiduo { get; set; }

    [Key]
    [StringLength(10)]
    public string IdCaracteristica { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCaracteristica")]
    [InverseProperty("TipoResiduoCaracteristicas")]
    public virtual Caracteristica IdCaracteristicaNavigation { get; set; } = null!;

    [ForeignKey("IdTipoResiduo")]
    [InverseProperty("TipoResiduoCaracteristicas")]
    public virtual TipoResiduo IdTipoResiduoNavigation { get; set; } = null!;
}
