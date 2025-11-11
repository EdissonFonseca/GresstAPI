using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdSolicitud", "Item")]
[Table("SolicitudDetalle")]
[Index("IdResiduo", Name = "IdxSolicitudDetalleResiduo")]
[Index("IdMaterial", Name = "idxSolicitudDetalleMaterial")]
public partial class SolicitudDetalle
{
    [Key]
    public long IdSolicitud { get; set; }

    [Key]
    public int Item { get; set; }

    public long? IdResiduo { get; set; }

    public long? IdResiduoOrigen { get; set; }

    public long IdMaterial { get; set; }

    [StringLength(255)]
    public string? Descripcion { get; set; }

    [StringLength(40)]
    public string? IdSolicitante { get; set; }

    public long? IdDepositoOrigen { get; set; }

    [StringLength(40)]
    public string? IdTransportador { get; set; }

    [StringLength(40)]
    public string? IdTransportadorSolicitud { get; set; }

    [StringLength(10)]
    public string? IdVehiculo { get; set; }

    [StringLength(10)]
    public string? IdVehiculoSolicitud { get; set; }

    [StringLength(40)]
    public string? IdProveedor { get; set; }

    [StringLength(40)]
    public string? IdProveedorSolicitud { get; set; }

    public long? IdDepositoDestino { get; set; }

    public long? IdDepositoDestinoSolicitud { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaFin { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaRecepcion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaProceso { get; set; }

    /// <summary>
    /// &apos;I&apos;nicio / &apos;V&apos;alidación / &apos;T&apos;ransporte / &apos;R&apos;ecepción / &apos;P&apos;rocesamiento / &apos;F&apos;inalización
    /// </summary>
    [StringLength(1)]
    public string IdEtapa { get; set; } = null!;

    /// <summary>
    /// &apos;I&apos;nicio / &apos;P&apos;laneación / &apos;E&apos;jecución / &apos;C&apos;ertificación / &apos;F&apos;inalización
    /// </summary>
    [StringLength(1)]
    public string IdFase { get; set; } = null!;

    public long? IdTratamiento { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PesoSolicitud { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? VolumenSolicitud { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? CantidadSolicitud { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    public long? IdEmbalaje { get; set; }

    public long? IdEmbalajeSolicitud { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? CantidadEmbalaje { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? CantidadEmbalajeSolicitud { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioEmbalaje { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioServicio { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioCompra { get; set; }

    public string? Soporte { get; set; }

    public bool Solicitado { get; set; }

    public bool? Recibido { get; set; }

    public bool? Aceptado { get; set; }

    public bool? Procesado { get; set; }

    public bool? Conciliado { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    [StringLength(50)]
    public string? Referencia2 { get; set; }

    public long? IdCausa { get; set; }

    public string? Notas { get; set; }

    [StringLength(255)]
    public string? Certificado { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDepositoDestino")]
    [InverseProperty("SolicitudDetalleIdDepositoDestinoNavigations")]
    public virtual Deposito? IdDepositoDestinoNavigation { get; set; }

    [ForeignKey("IdDepositoDestinoSolicitud")]
    [InverseProperty("SolicitudDetalleIdDepositoDestinoSolicitudNavigations")]
    public virtual Deposito? IdDepositoDestinoSolicitudNavigation { get; set; }

    [ForeignKey("IdDepositoOrigen")]
    [InverseProperty("SolicitudDetalleIdDepositoOrigenNavigations")]
    public virtual Deposito? IdDepositoOrigenNavigation { get; set; }

    [ForeignKey("IdEmbalaje")]
    [InverseProperty("SolicitudDetalleIdEmbalajeNavigations")]
    public virtual Embalaje? IdEmbalajeNavigation { get; set; }

    [ForeignKey("IdEmbalajeSolicitud")]
    [InverseProperty("SolicitudDetalleIdEmbalajeSolicitudNavigations")]
    public virtual Embalaje? IdEmbalajeSolicitudNavigation { get; set; }

    [ForeignKey("IdMaterial")]
    [InverseProperty("SolicitudDetalles")]
    public virtual Material IdMaterialNavigation { get; set; } = null!;

    [ForeignKey("IdProveedor")]
    [InverseProperty("SolicitudDetalleIdProveedorNavigations")]
    public virtual Persona? IdProveedorNavigation { get; set; }

    [ForeignKey("IdProveedorSolicitud")]
    [InverseProperty("SolicitudDetalleIdProveedorSolicitudNavigations")]
    public virtual Persona? IdProveedorSolicitudNavigation { get; set; }

    [ForeignKey("IdResiduo")]
    [InverseProperty("SolicitudDetalleIdResiduoNavigations")]
    public virtual Residuo? IdResiduoNavigation { get; set; }

    [ForeignKey("IdResiduoOrigen")]
    [InverseProperty("SolicitudDetalleIdResiduoOrigenNavigations")]
    public virtual Residuo? IdResiduoOrigenNavigation { get; set; }

    [ForeignKey("IdSolicitante")]
    [InverseProperty("SolicitudDetalleIdSolicitanteNavigations")]
    public virtual Persona? IdSolicitanteNavigation { get; set; }

    [ForeignKey("IdSolicitud")]
    [InverseProperty("SolicitudDetalles")]
    public virtual Solicitud IdSolicitudNavigation { get; set; } = null!;

    [ForeignKey("IdTransportador")]
    [InverseProperty("SolicitudDetalleIdTransportadorNavigations")]
    public virtual Persona? IdTransportadorNavigation { get; set; }

    [ForeignKey("IdTransportadorSolicitud")]
    [InverseProperty("SolicitudDetalleIdTransportadorSolicitudNavigations")]
    public virtual Persona? IdTransportadorSolicitudNavigation { get; set; }

    [ForeignKey("IdTratamiento")]
    [InverseProperty("SolicitudDetalles")]
    public virtual Tratamiento? IdTratamientoNavigation { get; set; }

    [ForeignKey("IdVehiculo")]
    [InverseProperty("SolicitudDetalleIdVehiculoNavigations")]
    public virtual Vehiculo? IdVehiculoNavigation { get; set; }

    [ForeignKey("IdVehiculoSolicitud")]
    [InverseProperty("SolicitudDetalleIdVehiculoSolicitudNavigations")]
    public virtual Vehiculo? IdVehiculoSolicitudNavigation { get; set; }

    [InverseProperty("SolicitudDetalle")]
    public virtual ICollection<SolicitudDetalleInsumo> SolicitudDetalleInsumos { get; set; } = new List<SolicitudDetalleInsumo>();

    [InverseProperty("SolicitudDetalle")]
    public virtual ICollection<SolicitudDetalleParticipacion> SolicitudDetalleParticipacions { get; set; } = new List<SolicitudDetalleParticipacion>();

    [InverseProperty("SolicitudDetalle")]
    public virtual ICollection<SolicitudDetalleServicio> SolicitudDetalleServicios { get; set; } = new List<SolicitudDetalleServicio>();
}
