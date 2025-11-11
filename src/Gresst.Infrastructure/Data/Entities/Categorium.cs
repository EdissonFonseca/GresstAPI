using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

public partial class Categorium
{
    [Key]
    [StringLength(20)]
    public string IdCategoria { get; set; } = null!;

    [StringLength(255)]
    public string? Nombre { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Caracteristica> Caracteristicas { get; set; } = new List<Caracteristica>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Clasificacion> Clasificacions { get; set; } = new List<Clasificacion>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Frase> Frases { get; set; } = new List<Frase>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Localizacion> Localizacions { get; set; } = new List<Localizacion>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Pictograma> Pictogramas { get; set; } = new List<Pictograma>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();
}
