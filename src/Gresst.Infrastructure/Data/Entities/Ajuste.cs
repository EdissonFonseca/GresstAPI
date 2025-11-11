using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Ajuste")]
public partial class Ajuste
{
    [Key]
    public long IdAjuste { get; set; }

    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    public long IdResiduo { get; set; }

    public long IdDeposito { get; set; }

    public long? IdDepositoDestino { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    public string? Notas { get; set; }

    public string? Soporte { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDepositoDestino")]
    [InverseProperty("AjusteIdDepositoDestinoNavigations")]
    public virtual Deposito? IdDepositoDestinoNavigation { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("AjusteIdDepositoNavigations")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("Ajustes")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdResiduo")]
    [InverseProperty("Ajustes")]
    public virtual Residuo IdResiduoNavigation { get; set; } = null!;
}
