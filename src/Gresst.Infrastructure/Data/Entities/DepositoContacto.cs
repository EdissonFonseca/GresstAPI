using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdDeposito", "IdRelacion", "IdContacto")]
[Table("Deposito_Contacto")]
public partial class DepositoContacto
{
    [Key]
    public long IdDeposito { get; set; }

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
    [InverseProperty("DepositoContactos")]
    public virtual Persona IdContactoNavigation { get; set; } = null!;

    [ForeignKey("IdDeposito")]
    [InverseProperty("DepositoContactos")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("DepositoContactos")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;
}
