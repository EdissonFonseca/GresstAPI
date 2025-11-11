using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdMaterial", "IdDeposito", "IdCuenta")]
[Table("Persona_Material_Deposito")]
public partial class PersonaMaterialDeposito
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdMaterial { get; set; }

    [Key]
    public long IdDeposito { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioServicio { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioCompra { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? FactorCompensacionEmision { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("PersonaMaterialDepositos")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdMaterial")]
    [InverseProperty("PersonaMaterialDepositos")]
    public virtual Material IdMaterialNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaMaterialDepositos")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
