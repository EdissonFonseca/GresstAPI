using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdOrden", "IdResiduo")]
[Table("OrdenResiduo")]
[Index("IdResiduo", Name = "IdxOrdenResiduoResiduo")]
public partial class OrdenResiduo
{
    [Key]
    public long IdOrden { get; set; }

    [Key]
    public long IdResiduo { get; set; }

    public long? IdTratamiento { get; set; }

    [StringLength(40)]
    public string? IdSolicitante { get; set; }

    public long? IdDepositoOrigen { get; set; }

    [StringLength(40)]
    public string? IdProveedor { get; set; }

    public long? IdDepositoDestino { get; set; }

    public long? IdResiduoDestino { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PesoOrden { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? VolumenOrden { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? CantidadOrden { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    public string? Soporte { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public string? Observaciones { get; set; }

    public long? IdCertificado { get; set; }

    public bool? Procesado { get; set; }

    public bool? Alterado { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaProceso { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCertificado")]
    [InverseProperty("OrdenResiduos")]
    public virtual Certificado? IdCertificadoNavigation { get; set; }

    [ForeignKey("IdDepositoDestino")]
    [InverseProperty("OrdenResiduoIdDepositoDestinoNavigations")]
    public virtual Deposito? IdDepositoDestinoNavigation { get; set; }

    [ForeignKey("IdDepositoOrigen")]
    [InverseProperty("OrdenResiduoIdDepositoOrigenNavigations")]
    public virtual Deposito? IdDepositoOrigenNavigation { get; set; }

    [ForeignKey("IdOrden")]
    [InverseProperty("OrdenResiduos")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [ForeignKey("IdProveedor")]
    [InverseProperty("OrdenResiduoIdProveedorNavigations")]
    public virtual Persona? IdProveedorNavigation { get; set; }

    [ForeignKey("IdResiduoDestino")]
    [InverseProperty("OrdenResiduoIdResiduoDestinoNavigations")]
    public virtual Residuo? IdResiduoDestinoNavigation { get; set; }

    [ForeignKey("IdResiduo")]
    [InverseProperty("OrdenResiduoIdResiduoNavigations")]
    public virtual Residuo IdResiduoNavigation { get; set; } = null!;

    [ForeignKey("IdSolicitante")]
    [InverseProperty("OrdenResiduoIdSolicitanteNavigations")]
    public virtual Persona? IdSolicitanteNavigation { get; set; }

    [ForeignKey("IdTratamiento")]
    [InverseProperty("OrdenResiduos")]
    public virtual Tratamiento? IdTratamientoNavigation { get; set; }

    [InverseProperty("OrdenResiduo")]
    public virtual ICollection<OrdenResiduoInsumo> OrdenResiduoInsumos { get; set; } = new List<OrdenResiduoInsumo>();

    [InverseProperty("OrdenResiduo")]
    public virtual ICollection<OrdenResiduoTransformacion> OrdenResiduoTransformacions { get; set; } = new List<OrdenResiduoTransformacion>();
}
