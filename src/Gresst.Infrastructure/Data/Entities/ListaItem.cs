using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdLista", "Item")]
[Table("ListaItem")]
public partial class ListaItem
{
    [Key]
    public long IdLista { get; set; }

    [Key]
    public int Item { get; set; }

    [StringLength(10)]
    public string? IdItem { get; set; }

    [StringLength(255)]
    public string? Descripcion { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdLista")]
    [InverseProperty("ListaItems")]
    public virtual Listum IdListaNavigation { get; set; } = null!;
}
