using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Relacion")]
public partial class Relacion
{
    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [StringLength(50)]
    public string? Nombre { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<ClasificacionItem> ClasificacionItems { get; set; } = new List<ClasificacionItem>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<DepositoContacto> DepositoContactos { get; set; } = new List<DepositoContacto>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<DepositoLocalizacion> DepositoLocalizacions { get; set; } = new List<DepositoLocalizacion>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<DepositoTipoResiduo> DepositoTipoResiduos { get; set; } = new List<DepositoTipoResiduo>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<DepositoVehiculo> DepositoVehiculos { get; set; } = new List<DepositoVehiculo>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<LocalizacionItem> LocalizacionItems { get; set; } = new List<LocalizacionItem>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<MaterialItem> MaterialItems { get; set; } = new List<MaterialItem>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<PersonaContacto> PersonaContactos { get; set; } = new List<PersonaContacto>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<PersonaLocalizacionContacto> PersonaLocalizacionContactos { get; set; } = new List<PersonaLocalizacionContacto>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<PersonaLocalizacionDeposito> PersonaLocalizacionDepositos { get; set; } = new List<PersonaLocalizacionDeposito>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<PersonaLocalizacion> PersonaLocalizacions { get; set; } = new List<PersonaLocalizacion>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<PersonaMaterialItem> PersonaMaterialItems { get; set; } = new List<PersonaMaterialItem>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<PersonaVehiculo> PersonaVehiculos { get; set; } = new List<PersonaVehiculo>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<ServicioItem> ServicioItems { get; set; } = new List<ServicioItem>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<TipoResiduoItem> TipoResiduoItems { get; set; } = new List<TipoResiduoItem>();

    [InverseProperty("IdRelacionNavigation")]
    public virtual ICollection<TratamientoItem> TratamientoItems { get; set; } = new List<TratamientoItem>();
}
