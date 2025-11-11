using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdMaterial", "IdTratamiento", "IdCuenta")]
[Table("Persona_Material_Tratamiento")]
public partial class PersonaMaterialTratamiento
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdMaterial { get; set; }

    [Key]
    public long IdTratamiento { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdMaterial")]
    [InverseProperty("PersonaMaterialTratamientos")]
    public virtual Material IdMaterialNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaMaterialTratamientos")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdTratamiento")]
    [InverseProperty("PersonaMaterialTratamientos")]
    public virtual Tratamiento IdTratamientoNavigation { get; set; } = null!;
}
