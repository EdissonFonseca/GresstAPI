using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Pictograma")]
public partial class Pictograma
{
    [Key]
    [StringLength(10)]
    public string IdPictograma { get; set; } = null!;

    [StringLength(500)]
    public string? Nombre { get; set; }

    [StringLength(50)]
    public string? Imagen { get; set; }

    [StringLength(20)]
    public string IdCategoria { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCategoria")]
    [InverseProperty("Pictogramas")]
    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    [InverseProperty("IdPictogramaNavigation")]
    public virtual ICollection<TipoResiduoPictograma> TipoResiduoPictogramas { get; set; } = new List<TipoResiduoPictograma>();
}
