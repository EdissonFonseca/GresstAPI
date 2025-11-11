using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdTipoResiduo", "IdPictograma")]
[Table("TipoResiduo_Pictograma")]
public partial class TipoResiduoPictograma
{
    [Key]
    public int IdTipoResiduo { get; set; }

    [Key]
    [StringLength(10)]
    public string IdPictograma { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPictograma")]
    [InverseProperty("TipoResiduoPictogramas")]
    public virtual Pictograma IdPictogramaNavigation { get; set; } = null!;

    [ForeignKey("IdTipoResiduo")]
    [InverseProperty("TipoResiduoPictogramas")]
    public virtual TipoResiduo IdTipoResiduoNavigation { get; set; } = null!;
}
