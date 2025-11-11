using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdDeposito", "IdRelacion", "IdLocalizacion")]
[Table("Deposito_Localizacion")]
public partial class DepositoLocalizacion
{
    [Key]
    public long IdDeposito { get; set; }

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    [StringLength(10)]
    public string IdLocalizacion { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("DepositoLocalizacions")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdLocalizacion")]
    [InverseProperty("DepositoLocalizacions")]
    public virtual Localizacion IdLocalizacionNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("DepositoLocalizacions")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;
}
