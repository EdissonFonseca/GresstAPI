using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Servicio")]
public partial class Servicio
{
    [Key]
    public long IdServicio { get; set; }

    [StringLength(50)]
    public string? Nombre { get; set; }

    [StringLength(20)]
    public string? IdCategoria { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<Gestion> Gestions { get; set; } = new List<Gestion>();

    [ForeignKey("IdCategoria")]
    [InverseProperty("Servicios")]
    public virtual Categorium? IdCategoriaNavigation { get; set; }

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<PersonaServicio> PersonaServicios { get; set; } = new List<PersonaServicio>();

    [InverseProperty("IdItemNavigation")]
    public virtual ICollection<ServicioItem> ServicioItemIdItemNavigations { get; set; } = new List<ServicioItem>();

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<ServicioItem> ServicioItemIdServicioNavigations { get; set; } = new List<ServicioItem>();

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<Solicitud> Solicituds { get; set; } = new List<Solicitud>();

    [InverseProperty("IdServicioNavigation")]
    public virtual ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();
}
