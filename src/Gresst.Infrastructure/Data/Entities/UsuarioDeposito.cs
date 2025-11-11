using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdUsuario", "IdDeposito")]
[Table("Usuario_Deposito")]
public partial class UsuarioDeposito
{
    [Key]
    public long IdUsuario { get; set; }

    [Key]
    public long IdDeposito { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("UsuarioDepositos")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("UsuarioDepositos")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
