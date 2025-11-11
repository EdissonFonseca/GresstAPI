using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdOrden", "IdSolicitud", "IdDeposito")]
[Table("OrdenPlaneacion")]
public partial class OrdenPlaneacion
{
    [Key]
    public long IdOrden { get; set; }

    [Key]
    public long IdSolicitud { get; set; }

    [Key]
    public long IdDeposito { get; set; }

    public Geometry? Ubicacion { get; set; }

    public int? Posicion { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Precio { get; set; }

    public string? Notas { get; set; }

    public string? Soporte { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    [StringLength(40)]
    public string? IdResponsable { get; set; }

    [StringLength(2)]
    public string? Horario { get; set; }

    public string? DatosSolicitante { get; set; }

    public string? DatosTransportador { get; set; }

    public string? DatosProveedor { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? FechaInicial { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaFinal { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? CantidadCombustible { get; set; }

    [StringLength(50)]
    public string? CargoTercero { get; set; }

    public string? FirmaTercero { get; set; }

    [StringLength(20)]
    public string? IdentifiicacionTercero { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Kilometraje { get; set; }

    [StringLength(50)]
    public string? NombreTercero { get; set; }

    public string? ObservacionesTercero { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdOrden")]
    [InverseProperty("OrdenPlaneacions")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [ForeignKey("IdResponsable")]
    [InverseProperty("OrdenPlaneacions")]
    public virtual Persona? IdResponsableNavigation { get; set; }

    [ForeignKey("IdSolicitud")]
    [InverseProperty("OrdenPlaneacions")]
    public virtual Solicitud IdSolicitudNavigation { get; set; } = null!;
}
