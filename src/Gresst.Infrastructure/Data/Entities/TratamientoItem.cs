using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdItem", "IdRelacion", "IdTratamiento")]
[Table("Tratamiento_Item")]
public partial class TratamientoItem
{
    [Key]
    public long IdItem { get; set; }

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    public long IdTratamiento { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdItem")]
    [InverseProperty("TratamientoItemIdItemNavigations")]
    public virtual Tratamiento IdItemNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("TratamientoItems")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;

    [ForeignKey("IdTratamiento")]
    [InverseProperty("TratamientoItemIdTratamientoNavigations")]
    public virtual Tratamiento IdTratamientoNavigation { get; set; } = null!;
}
