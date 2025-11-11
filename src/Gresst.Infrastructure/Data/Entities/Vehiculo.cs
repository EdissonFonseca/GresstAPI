using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Vehiculo")]
public partial class Vehiculo
{
    [Key]
    [StringLength(10)]
    public string IdVehiculo { get; set; } = null!;

    [StringLength(40)]
    public string? IdResponsable { get; set; }

    [StringLength(255)]
    public string? Descripcion { get; set; }

    public int? Modelo { get; set; }

    [Column(TypeName = "image")]
    public byte[]? Foto { get; set; }

    [StringLength(10)]
    public string? IdTipoVehiculo { get; set; }

    [StringLength(10)]
    public string? IdCarroceria { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Alto { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Largo { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Ancho { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cubicaje { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaRevision { get; set; }

    [Column("FechaSOAT", TypeName = "datetime")]
    public DateTime? FechaSoat { get; set; }

    public bool Activo { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<DepositoVehiculo> DepositoVehiculos { get; set; } = new List<DepositoVehiculo>();

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<Gestion> Gestions { get; set; } = new List<Gestion>();

    [ForeignKey("IdResponsable")]
    [InverseProperty("Vehiculos")]
    public virtual Persona? IdResponsableNavigation { get; set; }

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<PersonaVehiculo> PersonaVehiculos { get; set; } = new List<PersonaVehiculo>();

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<PlaneacionResponsable> PlaneacionResponsables { get; set; } = new List<PlaneacionResponsable>();

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<Rutum> Ruta { get; set; } = new List<Rutum>();

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdVehiculoNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdVehiculoSolicitudNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdVehiculoSolicitudNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<Solicitud> Solicituds { get; set; } = new List<Solicitud>();

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<UsuarioVehiculo> UsuarioVehiculos { get; set; } = new List<UsuarioVehiculo>();
}
