using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Persona")]
public partial class Persona
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [StringLength(20)]
    public string? IdCategoria { get; set; }

    public long? IdCuenta { get; set; }

    [StringLength(5)]
    public string? IdTipoIdentificacion { get; set; }

    [StringLength(2)]
    public string? IdRol { get; set; }

    [StringLength(3)]
    public string? IdTipoPersona { get; set; }

    [StringLength(20)]
    public string? Identificacion { get; set; }

    public int? DigitoVerificacion { get; set; }

    [StringLength(255)]
    public string? Nombre { get; set; }

    [StringLength(255)]
    public string? Direccion { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    [StringLength(50)]
    public string? Telefono2 { get; set; }

    [StringLength(100)]
    public string? Correo { get; set; }

    public Geometry? UbicacionMapa { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry? UbicacionLocal { get; set; }

    public bool Activo { get; set; }

    public string? Licencia { get; set; }

    [StringLength(255)]
    public string? Cargo { get; set; }

    [StringLength(255)]
    public string? Pagina { get; set; }

    [StringLength(255)]
    public string? Firma { get; set; }

    [StringLength(10)]
    public string? IdLocalizacion { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Ajuste> Ajustes { get; set; } = new List<Ajuste>();

    [InverseProperty("IdGeneradorNavigation")]
    public virtual ICollection<Certificado> CertificadoIdGeneradorNavigations { get; set; } = new List<Certificado>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Certificado> CertificadoIdPersonaNavigations { get; set; } = new List<Certificado>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<Certificado> CertificadoIdResponsableNavigations { get; set; } = new List<Certificado>();

    [InverseProperty("IdSolicitanteNavigation")]
    public virtual ICollection<Certificado> CertificadoIdSolicitanteNavigations { get; set; } = new List<Certificado>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<CertificadoPersona> CertificadoPersonas { get; set; } = new List<CertificadoPersona>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Cuentum> Cuenta { get; set; } = new List<Cuentum>();

    [InverseProperty("IdContactoNavigation")]
    public virtual ICollection<DepositoContacto> DepositoContactos { get; set; } = new List<DepositoContacto>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<Gestion> Gestions { get; set; } = new List<Gestion>();

    [ForeignKey("IdCategoria")]
    [InverseProperty("Personas")]
    public virtual Categorium? IdCategoriaNavigation { get; set; }

    [ForeignKey("IdCuenta")]
    [InverseProperty("Personas")]
    public virtual Cuentum? IdCuentaNavigation { get; set; }

    [ForeignKey("IdLocalizacion")]
    [InverseProperty("Personas")]
    public virtual Localizacion? IdLocalizacionNavigation { get; set; }

    [InverseProperty("IdPersonaEmisorNavigation")]
    public virtual ICollection<Mensaje> MensajeIdPersonaEmisorNavigations { get; set; } = new List<Mensaje>();

    [InverseProperty("IdPersonaReceptorNavigation")]
    public virtual ICollection<Mensaje> MensajeIdPersonaReceptorNavigations { get; set; } = new List<Mensaje>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Orden> OrdenIdPersonaNavigations { get; set; } = new List<Orden>();

    [InverseProperty("IdResponsable2Navigation")]
    public virtual ICollection<Orden> OrdenIdResponsable2Navigations { get; set; } = new List<Orden>();

    [InverseProperty("IdResponsable3Navigation")]
    public virtual ICollection<Orden> OrdenIdResponsable3Navigations { get; set; } = new List<Orden>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<Orden> OrdenIdResponsableNavigations { get; set; } = new List<Orden>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<OrdenPlaneacion> OrdenPlaneacions { get; set; } = new List<OrdenPlaneacion>();

    [InverseProperty("IdProveedorNavigation")]
    public virtual ICollection<OrdenResiduo> OrdenResiduoIdProveedorNavigations { get; set; } = new List<OrdenResiduo>();

    [InverseProperty("IdSolicitanteNavigation")]
    public virtual ICollection<OrdenResiduo> OrdenResiduoIdSolicitanteNavigations { get; set; } = new List<OrdenResiduo>();

    [InverseProperty("IdPropietarioNavigation")]
    public virtual ICollection<OrdenResiduoTransformacion> OrdenResiduoTransformacions { get; set; } = new List<OrdenResiduoTransformacion>();

    [InverseProperty("IdContactoNavigation")]
    public virtual ICollection<PersonaContacto> PersonaContactoIdContactoNavigations { get; set; } = new List<PersonaContacto>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaContacto> PersonaContactoIdPersonaNavigations { get; set; } = new List<PersonaContacto>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaEmbalaje> PersonaEmbalajes { get; set; } = new List<PersonaEmbalaje>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaInsumo> PersonaInsumos { get; set; } = new List<PersonaInsumo>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaLicencium> PersonaLicencia { get; set; } = new List<PersonaLicencium>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaLocalizacionArea> PersonaLocalizacionAreas { get; set; } = new List<PersonaLocalizacionArea>();

    [InverseProperty("IdContactoNavigation")]
    public virtual ICollection<PersonaLocalizacionContacto> PersonaLocalizacionContactos { get; set; } = new List<PersonaLocalizacionContacto>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaLocalizacionDeposito> PersonaLocalizacionDepositos { get; set; } = new List<PersonaLocalizacionDeposito>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaLocalizacion> PersonaLocalizacions { get; set; } = new List<PersonaLocalizacion>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaMaterialDepositoPrecio> PersonaMaterialDepositoPrecios { get; set; } = new List<PersonaMaterialDepositoPrecio>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaMaterialDeposito> PersonaMaterialDepositos { get; set; } = new List<PersonaMaterialDeposito>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaMaterialItem> PersonaMaterialItems { get; set; } = new List<PersonaMaterialItem>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaMaterialPrecio> PersonaMaterialPrecios { get; set; } = new List<PersonaMaterialPrecio>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaMaterialTratamiento> PersonaMaterialTratamientos { get; set; } = new List<PersonaMaterialTratamiento>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaMaterial> PersonaMaterials { get; set; } = new List<PersonaMaterial>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaServicio> PersonaServicios { get; set; } = new List<PersonaServicio>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaTipoResiduo> PersonaTipoResiduos { get; set; } = new List<PersonaTipoResiduo>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaTratamiento> PersonaTratamientos { get; set; } = new List<PersonaTratamiento>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<PersonaVehiculo> PersonaVehiculos { get; set; } = new List<PersonaVehiculo>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<PlaneacionResponsable> PlaneacionResponsables { get; set; } = new List<PlaneacionResponsable>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Referencium> ReferenciumIdPersonaNavigations { get; set; } = new List<Referencium>();

    [InverseProperty("IdResponsable2Navigation")]
    public virtual ICollection<Referencium> ReferenciumIdResponsable2Navigations { get; set; } = new List<Referencium>();

    [InverseProperty("IdResponsable3Navigation")]
    public virtual ICollection<Referencium> ReferenciumIdResponsable3Navigations { get; set; } = new List<Referencium>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<Referencium> ReferenciumIdResponsableNavigations { get; set; } = new List<Referencium>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();

    [InverseProperty("IdPropietarioNavigation")]
    public virtual ICollection<Residuo> Residuos { get; set; } = new List<Residuo>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<Rutum> Ruta { get; set; } = new List<Rutum>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<RutaResponsablePeriodo> RutaResponsablePeriodos { get; set; } = new List<RutaResponsablePeriodo>();

    [InverseProperty("IdProveedorNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdProveedorNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdProveedorSolicitudNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdProveedorSolicitudNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdSolicitanteNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdSolicitanteNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdTransportadorNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdTransportadorNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdTransportadorSolicitudNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdTransportadorSolicitudNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<SolicitudDetalleParticipacion> SolicitudDetalleParticipacions { get; set; } = new List<SolicitudDetalleParticipacion>();

    [InverseProperty("IdConductorNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdConductorNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdGeneradorNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdGeneradorNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdPersonaNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdProveedorNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdProveedorNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdResponsableNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdSolicitanteNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdSolicitanteNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdTransportadorNavigation")]
    public virtual ICollection<Solicitud> SolicitudIdTransportadorNavigations { get; set; } = new List<Solicitud>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<UsuarioPersona> UsuarioPersonas { get; set; } = new List<UsuarioPersona>();

    [InverseProperty("IdPersonaNavigation")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
