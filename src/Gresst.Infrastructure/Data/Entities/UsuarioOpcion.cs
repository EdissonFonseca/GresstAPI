using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdUsuario", "IdOpcion")]
[Table("Usuario_Opcion")]
public partial class UsuarioOpcion
{
    [Key]
    public long IdUsuario { get; set; }

    [Key]
    [StringLength(50)]
    public string IdOpcion { get; set; } = null!;

    public bool Habilitado { get; set; }

    [StringLength(255)]
    public string? Settings { get; set; }

    [StringLength(4)]
    public string? Permisos { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdOpcion")]
    [InverseProperty("UsuarioOpcions")]
    public virtual Opcion IdOpcionNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("UsuarioOpcions")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
