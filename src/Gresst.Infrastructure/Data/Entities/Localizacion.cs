using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Localizacion")]
public partial class Localizacion
{
    [Key]
    [StringLength(10)]
    public string IdLocalizacion { get; set; } = null!;

    [StringLength(20)]
    public string? IdCategoria { get; set; }

    [StringLength(50)]
    public string? Nombre { get; set; }

    [StringLength(100)]
    public string? Direccion { get; set; }

    public Geometry? Ubicacion { get; set; }

    public string? UbicacionDescripcion { get; set; }

    [StringLength(1)]
    public string? IdTipo { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdLocalizacionNavigation")]
    public virtual ICollection<DepositoLocalizacion> DepositoLocalizacions { get; set; } = new List<DepositoLocalizacion>();

    [InverseProperty("IdLocalizacionNavigation")]
    public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();

    [ForeignKey("IdCategoria")]
    [InverseProperty("Localizacions")]
    public virtual Categorium? IdCategoriaNavigation { get; set; }

    [InverseProperty("IdItemNavigation")]
    public virtual ICollection<LocalizacionItem> LocalizacionItemIdItemNavigations { get; set; } = new List<LocalizacionItem>();

    [InverseProperty("IdLocalizacionNavigation")]
    public virtual ICollection<LocalizacionItem> LocalizacionItemIdLocalizacionNavigations { get; set; } = new List<LocalizacionItem>();

    [InverseProperty("IdLocalizacionNavigation")]
    public virtual ICollection<PersonaContacto> PersonaContactos { get; set; } = new List<PersonaContacto>();

    [InverseProperty("IdLocalizacionNavigation")]
    public virtual ICollection<PersonaLocalizacion> PersonaLocalizacions { get; set; } = new List<PersonaLocalizacion>();

    [InverseProperty("IdLocalizacionNavigation")]
    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
