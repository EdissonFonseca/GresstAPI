using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdMaterial", "IdDeposito", "FechaInicio", "IdCuenta")]
[Table("Persona_Material_Deposito_Precio")]
public partial class PersonaMaterialDepositoPrecio
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

    [Key]
    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioServicio { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioCompra { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("PersonaMaterialDepositoPrecios")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdMaterial")]
    [InverseProperty("PersonaMaterialDepositoPrecios")]
    public virtual Material IdMaterialNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaMaterialDepositoPrecios")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
