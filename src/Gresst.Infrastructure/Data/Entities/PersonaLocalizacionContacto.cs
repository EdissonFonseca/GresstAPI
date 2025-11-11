using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdUbicacion", "IdRelacion", "IdContacto")]
[Table("Persona_Localizacion_Contacto")]
public partial class PersonaLocalizacionContacto
{
    [Key]
    [StringLength(20)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public int IdUbicacion { get; set; }

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    [StringLength(40)]
    public string IdContacto { get; set; } = null!;

    public string? Notas { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdContacto")]
    [InverseProperty("PersonaLocalizacionContactos")]
    public virtual Persona IdContactoNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("PersonaLocalizacionContactos")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;
}
