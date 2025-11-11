using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Deposito")]
public partial class Deposito
{
    [Key]
    public long IdDeposito { get; set; }

    [StringLength(40)]
    public string? IdPersona { get; set; }

    public int? IdUbicacion { get; set; }

    public long? IdCuenta { get; set; }

    public long? IdSuperior { get; set; }

    [StringLength(255)]
    public string? Nombre { get; set; }

    public Geometry? Ubicacion { get; set; }

    [StringLength(10)]
    public string? IdLocalizacion { get; set; }

    [StringLength(255)]
    public string? Direccion { get; set; }

    [StringLength(255)]
    public string? Correo { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public string? Notas { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    public bool Acopio { get; set; }

    public bool Almacenamiento { get; set; }

    public bool Disposicion { get; set; }

    public bool Entrega { get; set; }

    public bool Recepcion { get; set; }

    public bool Tratamiento { get; set; }

    public bool Activo { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdDepositoDestinoNavigation")]
    public virtual ICollection<Ajuste> AjusteIdDepositoDestinoNavigations { get; set; } = new List<Ajuste>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<Ajuste> AjusteIdDepositoNavigations { get; set; } = new List<Ajuste>();

    [InverseProperty("IdDepositoOrigenNavigation")]
    public virtual ICollection<Certificado> Certificados { get; set; } = new List<Certificado>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<DepositoContacto> DepositoContactos { get; set; } = new List<DepositoContacto>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<DepositoLocalizacion> DepositoLocalizacions { get; set; } = new List<DepositoLocalizacion>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<DepositoTipoResiduo> DepositoTipoResiduos { get; set; } = new List<DepositoTipoResiduo>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<DepositoVehiculo> DepositoVehiculos { get; set; } = new List<DepositoVehiculo>();

    [InverseProperty("IdDepositoDestinoNavigation")]
    public virtual ICollection<Gestion> GestionIdDepositoDestinoNavigations { get; set; } = new List<Gestion>();

    [InverseProperty("IdDepositoOrigenNavigation")]
    public virtual ICollection<Gestion> GestionIdDepositoOrigenNavigations { get; set; } = new List<Gestion>();

    [InverseProperty("IdPlantaNavigation")]
    public virtual ICollection<Gestion> GestionIdPlantaNavigations { get; set; } = new List<Gestion>();

    [ForeignKey("IdLocalizacion")]
    [InverseProperty("Depositos")]
    public virtual Localizacion? IdLocalizacionNavigation { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("Depositos")]
    public virtual Persona? IdPersonaNavigation { get; set; }

    [ForeignKey("IdSuperior")]
    [InverseProperty("InverseIdSuperiorNavigation")]
    public virtual Deposito? IdSuperiorNavigation { get; set; }

    [InverseProperty("IdSuperiorNavigation")]
    public virtual ICollection<Deposito> InverseIdSuperiorNavigation { get; set; } = new List<Deposito>();

    [InverseProperty("IdDepositoDestinoNavigation")]
    public virtual ICollection<Orden> OrdenIdDepositoDestinoNavigations { get; set; } = new List<Orden>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<Orden> OrdenIdDepositoNavigations { get; set; } = new List<Orden>();

    [InverseProperty("IdDepositoDestinoNavigation")]
    public virtual ICollection<OrdenResiduo> OrdenResiduoIdDepositoDestinoNavigations { get; set; } = new List<OrdenResiduo>();

    [InverseProperty("IdDepositoOrigenNavigation")]
    public virtual ICollection<OrdenResiduo> OrdenResiduoIdDepositoOrigenNavigations { get; set; } = new List<OrdenResiduo>();

    [InverseProperty("IdDepositoDestinoNavigation")]
    public virtual ICollection<OrdenResiduoTransformacion> OrdenResiduoTransformacions { get; set; } = new List<OrdenResiduoTransformacion>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<PersonaLicencium> PersonaLicencia { get; set; } = new List<PersonaLicencium>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<PersonaLocalizacionDeposito> PersonaLocalizacionDepositos { get; set; } = new List<PersonaLocalizacionDeposito>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<PersonaMaterialDepositoPrecio> PersonaMaterialDepositoPrecios { get; set; } = new List<PersonaMaterialDepositoPrecio>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<PersonaMaterialDeposito> PersonaMaterialDepositos { get; set; } = new List<PersonaMaterialDeposito>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<PlaneacionResponsable> PlaneacionResponsables { get; set; } = new List<PlaneacionResponsable>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<RutaDeposito> RutaDepositos { get; set; } = new List<RutaDeposito>();

    [InverseProperty("IdDepositoDestinoNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdDepositoDestinoNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdDepositoDestinoSolicitudNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdDepositoDestinoSolicitudNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdDepositoOrigenNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdDepositoOrigenNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdDepositoOrigenNavigation")]
    public virtual ICollection<SolicitudDetalleParticipacion> SolicitudDetalleParticipacions { get; set; } = new List<SolicitudDetalleParticipacion>();

    [InverseProperty("IdDepositoDestinoNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdDepositoDestinoNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdDepositoMedioNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdDepositoMedioNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdDepositoOrigenNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdDepositoOrigenNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdDepositoNavigation")]
    public virtual ICollection<UsuarioDeposito> UsuarioDepositos { get; set; } = new List<UsuarioDeposito>();
}
