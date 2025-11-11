using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Orden")]
[Index("NumeroOrden", "IdServicio", "IdPersona", Name = "idxNumeroOrden")]
[Index("IdServicio", "IdPersona", "IdDeposito", "IdTratamiento", "FechaInicio", Name = "idxOrdenServicio")]
public partial class Orden
{
    [Key]
    public long IdOrden { get; set; }

    public long NumeroOrden { get; set; }

    public long IdServicio { get; set; }

    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    public long? IdDeposito { get; set; }

    public long? IdDepositoDestino { get; set; }

    public long? IdTratamiento { get; set; }

    [StringLength(10)]
    public string? IdVehiculo { get; set; }

    [StringLength(40)]
    public string? IdResponsable { get; set; }

    [StringLength(40)]
    public string? IdResponsable2 { get; set; }

    [StringLength(40)]
    public string? IdResponsable3 { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaFin { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaInicioProgramada { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PesoVacio { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PesoLleno { get; set; }

    public string? Notas { get; set; }

    public string? Soporte { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaInicial { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaFinal { get; set; }

    public Geometry? UbicacionInicial { get; set; }

    public Geometry? UbicacionFinal { get; set; }

    public string? DatosSolicitante { get; set; }

    public string? DatosTransportador { get; set; }

    public string? DatosProveedor { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public string? DatosAdicionales { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? CantidadCombustibleInicial { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? CantidadCombustibleFinal { get; set; }

    [StringLength(50)]
    public string? CargoTercero { get; set; }

    public string? FirmaTercero { get; set; }

    [StringLength(20)]
    public string? IdentifiicacionTercero { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? KilometrajeInicial { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? KilometrajeFinal { get; set; }

    [StringLength(50)]
    public string? NombreTercero { get; set; }

    public string? ObservacionesTercero { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<Certificado> Certificados { get; set; } = new List<Certificado>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<Gestion> Gestions { get; set; } = new List<Gestion>();

    [ForeignKey("IdDepositoDestino")]
    [InverseProperty("OrdenIdDepositoDestinoNavigations")]
    public virtual Deposito? IdDepositoDestinoNavigation { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("OrdenIdDepositoNavigations")]
    public virtual Deposito? IdDepositoNavigation { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("OrdenIdPersonaNavigations")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdResponsable2")]
    [InverseProperty("OrdenIdResponsable2Navigations")]
    public virtual Persona? IdResponsable2Navigation { get; set; }

    [ForeignKey("IdResponsable3")]
    [InverseProperty("OrdenIdResponsable3Navigations")]
    public virtual Persona? IdResponsable3Navigation { get; set; }

    [ForeignKey("IdResponsable")]
    [InverseProperty("OrdenIdResponsableNavigations")]
    public virtual Persona? IdResponsableNavigation { get; set; }

    [ForeignKey("IdServicio")]
    [InverseProperty("Ordens")]
    public virtual Servicio IdServicioNavigation { get; set; } = null!;

    [ForeignKey("IdTratamiento")]
    [InverseProperty("Ordens")]
    public virtual Tratamiento? IdTratamientoNavigation { get; set; }

    [ForeignKey("IdVehiculo")]
    [InverseProperty("Ordens")]
    public virtual Vehiculo? IdVehiculoNavigation { get; set; }

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<OrdenInsumo> OrdenInsumos { get; set; } = new List<OrdenInsumo>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<OrdenPlaneacion> OrdenPlaneacions { get; set; } = new List<OrdenPlaneacion>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<OrdenResiduoInsumo> OrdenResiduoInsumos { get; set; } = new List<OrdenResiduoInsumo>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<OrdenResiduoTransformacion> OrdenResiduoTransformacions { get; set; } = new List<OrdenResiduoTransformacion>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<OrdenResiduo> OrdenResiduos { get; set; } = new List<OrdenResiduo>();
}
