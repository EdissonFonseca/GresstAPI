using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdMaterial", "IdCuenta")]
[Table("Persona_Material")]
public partial class PersonaMaterial
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdMaterial { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    [StringLength(100)]
    public string? Nombre { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioServicio { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioCompra { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioVenta { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? FactorCompensacionEmision { get; set; }

    public bool Activo { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public long? IdEmbalaje { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdEmbalaje")]
    [InverseProperty("PersonaMaterials")]
    public virtual Embalaje? IdEmbalajeNavigation { get; set; }

    [ForeignKey("IdMaterial")]
    [InverseProperty("PersonaMaterials")]
    public virtual Material IdMaterialNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaMaterials")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
