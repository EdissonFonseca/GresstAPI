using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdItem", "IdRelacion", "IdLocalizacion")]
[Table("Localizacion_Item")]
public partial class LocalizacionItem
{
    [Key]
    [StringLength(10)]
    public string IdItem { get; set; } = null!;

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

    [ForeignKey("IdItem")]
    [InverseProperty("LocalizacionItemIdItemNavigations")]
    public virtual Localizacion IdItemNavigation { get; set; } = null!;

    [ForeignKey("IdLocalizacion")]
    [InverseProperty("LocalizacionItemIdLocalizacionNavigations")]
    public virtual Localizacion IdLocalizacionNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("LocalizacionItems")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;
}
