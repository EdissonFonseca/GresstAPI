using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdContacto", "IdRelacion", "IdPersona", "IdCuenta")]
[Table("Persona_Contacto")]
public partial class PersonaContacto
{
    [Key]
    [StringLength(40)]
    public string IdContacto { get; set; } = null!;

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdCuenta { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaFin { get; set; }

    [StringLength(1)]
    public string? IdEstado { get; set; }

    public bool? RequiereConciliar { get; set; }

    public bool? EnviarCorreo { get; set; }

    [StringLength(255)]
    public string? Correo { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    [StringLength(50)]
    public string? Telefono2 { get; set; }

    [StringLength(255)]
    public string? Direccion { get; set; }

    [StringLength(255)]
    public string? Nombre { get; set; }

    [StringLength(255)]
    public string? Cargo { get; set; }

    [StringLength(255)]
    public string? Pagina { get; set; }

    [StringLength(255)]
    public string? Firma { get; set; }

    [StringLength(10)]
    public string? IdLocalizacion { get; set; }

    public string? Notas { get; set; }

    public bool Activo { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdContacto")]
    [InverseProperty("PersonaContactoIdContactoNavigations")]
    public virtual Persona IdContactoNavigation { get; set; } = null!;

    [ForeignKey("IdCuenta")]
    [InverseProperty("PersonaContactos")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    [ForeignKey("IdLocalizacion")]
    [InverseProperty("PersonaContactos")]
    public virtual Localizacion? IdLocalizacionNavigation { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaContactoIdPersonaNavigations")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("PersonaContactos")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;
}
