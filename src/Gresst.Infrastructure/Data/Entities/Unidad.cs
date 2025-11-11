using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Unidad")]
public partial class Unidad
{
    [Key]
    public int IdUnidad { get; set; }

    [StringLength(10)]
    public string? Simbolo { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [StringLength(20)]
    public string? IdCategoria { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdUnidadNavigation")]
    public virtual ICollection<OrdenInsumo> OrdenInsumos { get; set; } = new List<OrdenInsumo>();

    [InverseProperty("IdUnidadNavigation")]
    public virtual ICollection<OrdenResiduoInsumo> OrdenResiduoInsumos { get; set; } = new List<OrdenResiduoInsumo>();

    [InverseProperty("IdUnidadNavigation")]
    public virtual ICollection<SolicitudDetalleInsumo> SolicitudDetalleInsumos { get; set; } = new List<SolicitudDetalleInsumo>();

    [InverseProperty("IdUnidadNavigation")]
    public virtual ICollection<SolicitudInsumo> SolicitudInsumos { get; set; } = new List<SolicitudInsumo>();
}
