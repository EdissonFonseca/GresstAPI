using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Insumo")]
public partial class Insumo
{
    [Key]
    public long IdInsumo { get; set; }

    public long? IdInsumoSuperior { get; set; }

    [StringLength(20)]
    public string IdCategoriaUnidad { get; set; } = null!;

    [StringLength(50)]
    public string? Nombre { get; set; }

    public bool Activo { get; set; }

    public bool Publico { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdInsumoSuperior")]
    [InverseProperty("InverseIdInsumoSuperiorNavigation")]
    public virtual Insumo? IdInsumoSuperiorNavigation { get; set; }

    [InverseProperty("IdInsumoSuperiorNavigation")]
    public virtual ICollection<Insumo> InverseIdInsumoSuperiorNavigation { get; set; } = new List<Insumo>();

    [InverseProperty("IdInsumoNavigation")]
    public virtual ICollection<OrdenInsumo> OrdenInsumos { get; set; } = new List<OrdenInsumo>();

    [InverseProperty("IdInsumoNavigation")]
    public virtual ICollection<OrdenResiduoInsumo> OrdenResiduoInsumos { get; set; } = new List<OrdenResiduoInsumo>();

    [InverseProperty("IdInsumoNavigation")]
    public virtual ICollection<PersonaInsumo> PersonaInsumos { get; set; } = new List<PersonaInsumo>();

    [InverseProperty("IdInsumoNavigation")]
    public virtual ICollection<SolicitudDetalleInsumo> SolicitudDetalleInsumos { get; set; } = new List<SolicitudDetalleInsumo>();

    [InverseProperty("IdInsumoNavigation")]
    public virtual ICollection<SolicitudInsumo> SolicitudInsumos { get; set; } = new List<SolicitudInsumo>();
}
