using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Gestion")]
public partial class Gestion
{
    [Key]
    public long IdMovimiento { get; set; }

    public long IdResiduo { get; set; }

    public long IdDepositoOrigen { get; set; }

    public long? IdPlanta { get; set; }

    [StringLength(10)]
    public string? IdVehiculo { get; set; }

    public long? IdDepositoDestino { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Fecha { get; set; }

    public long IdServicio { get; set; }

    public long? IdTratamiento { get; set; }

    [StringLength(40)]
    public string? IdResponsable { get; set; }

    public bool Procesado { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? Porcentaje { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? Cantidad { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal? Volumen { get; set; }

    public long? IdOrden { get; set; }

    public long? IdCertificado { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }

    [StringLength(500)]
    public string? Soportes { get; set; }

    public long? IdMovimientoOrigen { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [ForeignKey("IdCertificado")]
    [InverseProperty("Gestions")]
    public virtual Certificado? IdCertificadoNavigation { get; set; }

    [ForeignKey("IdDepositoDestino")]
    [InverseProperty("GestionIdDepositoDestinoNavigations")]
    public virtual Deposito? IdDepositoDestinoNavigation { get; set; }

    [ForeignKey("IdDepositoOrigen")]
    [InverseProperty("GestionIdDepositoOrigenNavigations")]
    public virtual Deposito IdDepositoOrigenNavigation { get; set; } = null!;

    [ForeignKey("IdMovimientoOrigen")]
    [InverseProperty("InverseIdMovimientoOrigenNavigation")]
    public virtual Gestion? IdMovimientoOrigenNavigation { get; set; }

    [ForeignKey("IdOrden")]
    [InverseProperty("Gestions")]
    public virtual Orden? IdOrdenNavigation { get; set; }

    [ForeignKey("IdPlanta")]
    [InverseProperty("GestionIdPlantaNavigations")]
    public virtual Deposito? IdPlantaNavigation { get; set; }

    [ForeignKey("IdResiduo")]
    [InverseProperty("Gestions")]
    public virtual Residuo IdResiduoNavigation { get; set; } = null!;

    [ForeignKey("IdResponsable")]
    [InverseProperty("Gestions")]
    public virtual Persona? IdResponsableNavigation { get; set; }

    [ForeignKey("IdServicio")]
    [InverseProperty("Gestions")]
    public virtual Servicio IdServicioNavigation { get; set; } = null!;

    [ForeignKey("IdTratamiento")]
    [InverseProperty("Gestions")]
    public virtual Tratamiento? IdTratamientoNavigation { get; set; }

    [ForeignKey("IdUsuarioCreacion")]
    [InverseProperty("GestionIdUsuarioCreacionNavigations")]
    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioUltimaModificacion")]
    [InverseProperty("GestionIdUsuarioUltimaModificacionNavigations")]
    public virtual Usuario? IdUsuarioUltimaModificacionNavigation { get; set; }

    [ForeignKey("IdVehiculo")]
    [InverseProperty("Gestions")]
    public virtual Vehiculo? IdVehiculoNavigation { get; set; }

    [InverseProperty("IdMovimientoOrigenNavigation")]
    public virtual ICollection<Gestion> InverseIdMovimientoOrigenNavigation { get; set; } = new List<Gestion>();
}
