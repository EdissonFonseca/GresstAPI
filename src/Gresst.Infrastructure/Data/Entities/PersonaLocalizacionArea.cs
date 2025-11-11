using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdUbicacion", "IdLocalizacion")]
[Table("Persona_Localizacion_Area")]
public partial class PersonaLocalizacionArea
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public int IdUbicacion { get; set; }

    [Key]
    [StringLength(10)]
    public string IdLocalizacion { get; set; } = null!;

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaLocalizacionAreas")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
