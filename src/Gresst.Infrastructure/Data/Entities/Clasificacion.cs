using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Clasificacion")]
public partial class Clasificacion
{
    [Key]
    [StringLength(10)]
    public string IdClasificacion { get; set; } = null!;

    [StringLength(500)]
    public string? Nombre { get; set; }

    [StringLength(20)]
    public string IdCategoria { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdClasificacionNavigation")]
    public virtual ICollection<ClasificacionItem> ClasificacionItemIdClasificacionNavigations { get; set; } = new List<ClasificacionItem>();

    [InverseProperty("IdItemNavigation")]
    public virtual ICollection<ClasificacionItem> ClasificacionItemIdItemNavigations { get; set; } = new List<ClasificacionItem>();

    [ForeignKey("IdCategoria")]
    [InverseProperty("Clasificacions")]
    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    [InverseProperty("IdClasificacionNavigation")]
    public virtual ICollection<TipoResiduoClasificacion> TipoResiduoClasificacions { get; set; } = new List<TipoResiduoClasificacion>();
}
