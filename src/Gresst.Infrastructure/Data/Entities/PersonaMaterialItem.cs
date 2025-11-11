using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdMaterial", "IdRelacion", "IdItem", "IdCuenta")]
[Table("Persona_Material_Item")]
public partial class PersonaMaterialItem
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdMaterial { get; set; }

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    public long IdItem { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    [Column(TypeName = "numeric(5, 2)")]
    public decimal? Porcentaje { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdItem")]
    [InverseProperty("PersonaMaterialItemIdItemNavigations")]
    public virtual Material IdItemNavigation { get; set; } = null!;

    [ForeignKey("IdMaterial")]
    [InverseProperty("PersonaMaterialItemIdMaterialNavigations")]
    public virtual Material IdMaterialNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaMaterialItems")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("PersonaMaterialItems")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;
}
