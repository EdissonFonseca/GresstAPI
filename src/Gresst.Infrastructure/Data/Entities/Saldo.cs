using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdResiduo", "IdDeposito")]
[Table("Saldo")]
[Index("IdResiduo", Name = "IdxSaldoResiduo")]
public partial class Saldo
{
    [Key]
    public long IdResiduo { get; set; }

    [Key]
    public long IdDeposito { get; set; }

    public long? IdTratamiento { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Precio { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdResiduo")]
    [InverseProperty("Saldos")]
    public virtual Residuo IdResiduoNavigation { get; set; } = null!;

    [ForeignKey("IdTratamiento")]
    [InverseProperty("Saldos")]
    public virtual Tratamiento? IdTratamientoNavigation { get; set; }
}
