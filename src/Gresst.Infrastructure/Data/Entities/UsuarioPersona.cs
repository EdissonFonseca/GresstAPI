using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdUsuario", "IdPersona")]
[Table("Usuario_Persona")]
public partial class UsuarioPersona
{
    [Key]
    public long IdUsuario { get; set; }

    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("UsuarioPersonas")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("UsuarioPersonas")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
