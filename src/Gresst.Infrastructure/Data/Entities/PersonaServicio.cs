using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdServicio", "FechaInicio", "IdCuenta")]
[Table("Persona_Servicio")]
public partial class PersonaServicio
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdServicio { get; set; }

    [Key]
    public DateOnly FechaInicio { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    public DateOnly? FechaFin { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaServicios")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdServicio")]
    [InverseProperty("PersonaServicios")]
    public virtual Servicio IdServicioNavigation { get; set; } = null!;
}
