using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Material")]
public partial class Material
{
    [Key]
    public long IdMaterial { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Sinonimos { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public int? IdTipoResiduo { get; set; }

    [StringLength(50)]
    public string? Imagen { get; set; }

    [StringLength(1)]
    public string? Medicion { get; set; }

    public bool Publico { get; set; }

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

    public bool Aprovechable { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdTipoResiduo")]
    [InverseProperty("Materials")]
    public virtual TipoResiduo? IdTipoResiduoNavigation { get; set; }

    [InverseProperty("IdItemNavigation")]
    public virtual ICollection<MaterialItem> MaterialItemIdItemNavigations { get; set; } = new List<MaterialItem>();

    [InverseProperty("IdMaterialNavigation")]
    public virtual ICollection<MaterialItem> MaterialItemIdMaterialNavigations { get; set; } = new List<MaterialItem>();

    [InverseProperty("IdMaterialNavigation")]
    public virtual ICollection<PersonaMaterialDepositoPrecio> PersonaMaterialDepositoPrecios { get; set; } = new List<PersonaMaterialDepositoPrecio>();

    [InverseProperty("IdMaterialNavigation")]
    public virtual ICollection<PersonaMaterialDeposito> PersonaMaterialDepositos { get; set; } = new List<PersonaMaterialDeposito>();

    [InverseProperty("IdItemNavigation")]
    public virtual ICollection<PersonaMaterialItem> PersonaMaterialItemIdItemNavigations { get; set; } = new List<PersonaMaterialItem>();

    [InverseProperty("IdMaterialNavigation")]
    public virtual ICollection<PersonaMaterialItem> PersonaMaterialItemIdMaterialNavigations { get; set; } = new List<PersonaMaterialItem>();

    [InverseProperty("IdMaterialNavigation")]
    public virtual ICollection<PersonaMaterialPrecio> PersonaMaterialPrecios { get; set; } = new List<PersonaMaterialPrecio>();

    [InverseProperty("IdMaterialNavigation")]
    public virtual ICollection<PersonaMaterialTratamiento> PersonaMaterialTratamientos { get; set; } = new List<PersonaMaterialTratamiento>();

    [InverseProperty("IdMaterialNavigation")]
    public virtual ICollection<PersonaMaterial> PersonaMaterials { get; set; } = new List<PersonaMaterial>();

    [InverseProperty("IdMaterialNavigation")]
    public virtual ICollection<Residuo> Residuos { get; set; } = new List<Residuo>();

    [InverseProperty("IdMaterialNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalles { get; set; } = new List<SolicitudDetalle>();
}
