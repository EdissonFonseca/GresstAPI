using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdUbicacion", "IdCuenta")]
[Table("Persona_Localizacion")]
public partial class PersonaLocalizacion
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public int IdUbicacion { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [StringLength(10)]
    public string IdLocalizacion { get; set; } = null!;

    [StringLength(255)]
    public string? Nombre { get; set; }

    [StringLength(255)]
    public string? Direccion { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public string? Notas { get; set; }

    public bool Activo { get; set; }

    [StringLength(255)]
    public string? Correo { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdLocalizacion")]
    [InverseProperty("PersonaLocalizacions")]
    public virtual Localizacion IdLocalizacionNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaLocalizacions")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("PersonaLocalizacions")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;
}
