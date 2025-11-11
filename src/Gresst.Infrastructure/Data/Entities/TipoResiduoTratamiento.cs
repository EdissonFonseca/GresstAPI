using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdTipoResiduo", "IdTratamiento")]
[Table("TipoResiduo_Tratamiento")]
public partial class TipoResiduoTratamiento
{
    [Key]
    public int IdTipoResiduo { get; set; }

    [Key]
    public long IdTratamiento { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdTipoResiduo")]
    [InverseProperty("TipoResiduoTratamientos")]
    public virtual TipoResiduo IdTipoResiduoNavigation { get; set; } = null!;

    [ForeignKey("IdTratamiento")]
    [InverseProperty("TipoResiduoTratamientos")]
    public virtual Tratamiento IdTratamientoNavigation { get; set; } = null!;
}
