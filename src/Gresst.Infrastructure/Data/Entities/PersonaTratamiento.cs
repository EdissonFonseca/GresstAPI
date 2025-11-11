using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdTratamiento", "IdCuenta")]
[Table("Persona_Tratamiento")]
public partial class PersonaTratamiento
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdTratamiento { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    public bool Activo { get; set; }

    public bool Manejado { get; set; }

    public bool Transferido { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaTratamientos")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdTratamiento")]
    [InverseProperty("PersonaTratamientos")]
    public virtual Tratamiento IdTratamientoNavigation { get; set; } = null!;
}
