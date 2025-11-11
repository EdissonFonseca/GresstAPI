using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdTipoResiduo", "IdCuenta")]
[Table("Persona_TipoResiduo")]
public partial class PersonaTipoResiduo
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public int IdTipoResiduo { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaTipoResiduos")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdTipoResiduo")]
    [InverseProperty("PersonaTipoResiduos")]
    public virtual TipoResiduo IdTipoResiduoNavigation { get; set; } = null!;
}
