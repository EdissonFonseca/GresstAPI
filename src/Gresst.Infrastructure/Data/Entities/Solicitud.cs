using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Solicitud")]
[Index("NumeroSolicitud", "IdServicio", "IdProveedor", Name = "idxNumeroSolicitud")]
[Index("IdPersona", Name = "idxSolicitudPersona")]
public partial class Solicitud
{
    [Key]
    public long IdSolicitud { get; set; }

    public long? NumeroSolicitud { get; set; }

    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    public long IdServicio { get; set; }

    [StringLength(40)]
    public string? IdSolicitante { get; set; }

    [StringLength(40)]
    public string? IdTransportador { get; set; }

    [StringLength(40)]
    public string? IdProveedor { get; set; }

    [StringLength(40)]
    public string? IdGenerador { get; set; }

    public long? IdDepositoOrigen { get; set; }

    public long? IdDepositoMedio { get; set; }

    public long? IdDepositoDestino { get; set; }

    [StringLength(10)]
    public string? IdVehiculo { get; set; }

    [StringLength(40)]
    public string? IdConductor { get; set; }

    [StringLength(40)]
    public string? IdResponsable { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaFin { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    public string? Recurrencia { get; set; }

    public string? Recordatorio { get; set; }

    public int? Ocurrencia { get; set; }

    public string? Notas { get; set; }

    public string? Soporte { get; set; }

    public bool? Facturada { get; set; }

    public bool? Pagada { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    [StringLength(50)]
    public string? Referencia2 { get; set; }

    public bool MultiplesGeneradores { get; set; }

    public bool MultiplesOrigenes { get; set; }

    public bool DistribuirPorParticipacion { get; set; }

    public bool MultiplesTransportadores { get; set; }

    public bool MultiplesVehiculos { get; set; }

    public bool MultiplesGestores { get; set; }

    public bool MultiplesDestinos { get; set; }

    public string? DatosSolicitante { get; set; }

    public string? DatosTransportador { get; set; }

    public string? DatosProveedor { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdSolicitudNavigation")]
    public virtual ICollection<Certificado> Certificados { get; set; } = new List<Certificado>();

    [ForeignKey("IdConductor")]
    [InverseProperty("SolicitudIdConductorNavigations")]
    public virtual Persona? IdConductorNavigation { get; set; }

    [ForeignKey("IdDepositoDestino")]
    [InverseProperty("SolicitudIdDepositoDestinoNavigations")]
    public virtual Deposito? IdDepositoDestinoNavigation { get; set; }

    [ForeignKey("IdDepositoMedio")]
    [InverseProperty("SolicitudIdDepositoMedioNavigations")]
    public virtual Deposito? IdDepositoMedioNavigation { get; set; }

    [ForeignKey("IdDepositoOrigen")]
    [InverseProperty("SolicitudIdDepositoOrigenNavigations")]
    public virtual Deposito? IdDepositoOrigenNavigation { get; set; }

    [ForeignKey("IdGenerador")]
    [InverseProperty("SolicitudIdGeneradorNavigations")]
    public virtual Persona? IdGeneradorNavigation { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("SolicitudIdPersonaNavigations")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdProveedor")]
    [InverseProperty("SolicitudIdProveedorNavigations")]
    public virtual Persona? IdProveedorNavigation { get; set; }

    [ForeignKey("IdResponsable")]
    [InverseProperty("SolicitudIdResponsableNavigations")]
    public virtual Persona? IdResponsableNavigation { get; set; }

    [ForeignKey("IdServicio")]
    [InverseProperty("Solicituds")]
    public virtual Servicio IdServicioNavigation { get; set; } = null!;

    [ForeignKey("IdSolicitante")]
    [InverseProperty("SolicitudIdSolicitanteNavigations")]
    public virtual Persona? IdSolicitanteNavigation { get; set; }

    [ForeignKey("IdTransportador")]
    [InverseProperty("SolicitudIdTransportadorNavigations")]
    public virtual Persona? IdTransportadorNavigation { get; set; }

    [ForeignKey("IdVehiculo")]
    [InverseProperty("Solicituds")]
    public virtual Vehiculo? IdVehiculoNavigation { get; set; }

    [InverseProperty("IdSolicitudNavigation")]
    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();

    [InverseProperty("IdSolicitudNavigation")]
    public virtual ICollection<OrdenPlaneacion> OrdenPlaneacions { get; set; } = new List<OrdenPlaneacion>();

    [InverseProperty("IdSolicitudNavigation")]
    public virtual ICollection<SolicitudDetalleInsumo> SolicitudDetalleInsumos { get; set; } = new List<SolicitudDetalleInsumo>();

    [InverseProperty("IdSolicitudNavigation")]
    public virtual ICollection<SolicitudDetalleServicio> SolicitudDetalleServicios { get; set; } = new List<SolicitudDetalleServicio>();

    [InverseProperty("IdSolicitudNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalles { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdSolicitudNavigation")]
    public virtual ICollection<SolicitudInsumo> SolicitudInsumos { get; set; } = new List<SolicitudInsumo>();
}
