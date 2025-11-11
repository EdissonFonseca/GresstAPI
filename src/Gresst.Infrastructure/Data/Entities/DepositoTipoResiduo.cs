using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdDeposito", "IdRelacion", "IdTipoResiduo")]
[Table("Deposito_TipoResiduo")]
public partial class DepositoTipoResiduo
{
    [Key]
    public long IdDeposito { get; set; }

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    public int IdTipoResiduo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("DepositoTipoResiduos")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("DepositoTipoResiduos")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;

    [ForeignKey("IdTipoResiduo")]
    [InverseProperty("DepositoTipoResiduos")]
    public virtual TipoResiduo IdTipoResiduoNavigation { get; set; } = null!;
}
