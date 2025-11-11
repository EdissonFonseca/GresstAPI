using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdEmbalaje", "IdCuenta")]
[Table("Persona_Embalaje")]
public partial class PersonaEmbalaje
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdEmbalaje { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Precio { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCuenta")]
    [InverseProperty("PersonaEmbalajes")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    [ForeignKey("IdEmbalaje")]
    [InverseProperty("PersonaEmbalajes")]
    public virtual Embalaje IdEmbalajeNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaEmbalajes")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
