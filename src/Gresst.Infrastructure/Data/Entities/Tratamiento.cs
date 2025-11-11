using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Tratamiento")]
public partial class Tratamiento
{
    [Key]
    public long IdTratamiento { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [StringLength(20)]
    public string? IdCategoria { get; set; }

    public bool? Publico { get; set; }

    public long IdServicio { get; set; }

    public bool Aprovechamiento { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<Gestion> Gestions { get; set; } = new List<Gestion>();

    [ForeignKey("IdCategoria")]
    [InverseProperty("Tratamientos")]
    public virtual Categorium? IdCategoriaNavigation { get; set; }

    [ForeignKey("IdServicio")]
    [InverseProperty("Tratamientos")]
    public virtual Servicio IdServicioNavigation { get; set; } = null!;

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<OrdenResiduoTransformacion> OrdenResiduoTransformacions { get; set; } = new List<OrdenResiduoTransformacion>();

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<OrdenResiduo> OrdenResiduos { get; set; } = new List<OrdenResiduo>();

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<PersonaMaterialTratamiento> PersonaMaterialTratamientos { get; set; } = new List<PersonaMaterialTratamiento>();

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<PersonaTratamiento> PersonaTratamientos { get; set; } = new List<PersonaTratamiento>();

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<Saldo> Saldos { get; set; } = new List<Saldo>();

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalles { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<TipoResiduoTratamiento> TipoResiduoTratamientos { get; set; } = new List<TipoResiduoTratamiento>();

    [InverseProperty("IdItemNavigation")]
    public virtual ICollection<TratamientoItem> TratamientoItemIdItemNavigations { get; set; } = new List<TratamientoItem>();

    [InverseProperty("IdTratamientoNavigation")]
    public virtual ICollection<TratamientoItem> TratamientoItemIdTratamientoNavigations { get; set; } = new List<TratamientoItem>();
}
