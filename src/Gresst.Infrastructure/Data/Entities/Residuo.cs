using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Residuo")]
public partial class Residuo
{
    [Key]
    public long IdResiduo { get; set; }

    public long IdMaterial { get; set; }

    [StringLength(40)]
    public string? IdPropietario { get; set; }

    [StringLength(255)]
    public string? Descripcion { get; set; }

    public string? Soporte { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? FechaIngreso { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdResiduoNavigation")]
    public virtual ICollection<Ajuste> Ajustes { get; set; } = new List<Ajuste>();

    [InverseProperty("IdResiduoNavigation")]
    public virtual ICollection<CertificadoResiduo> CertificadoResiduos { get; set; } = new List<CertificadoResiduo>();

    [InverseProperty("IdResiduoNavigation")]
    public virtual ICollection<Gestion> Gestions { get; set; } = new List<Gestion>();

    [ForeignKey("IdMaterial")]
    [InverseProperty("Residuos")]
    public virtual Material IdMaterialNavigation { get; set; } = null!;

    [ForeignKey("IdPropietario")]
    [InverseProperty("Residuos")]
    public virtual Persona? IdPropietarioNavigation { get; set; }

    [InverseProperty("IdResiduoDestinoNavigation")]
    public virtual ICollection<OrdenResiduo> OrdenResiduoIdResiduoDestinoNavigations { get; set; } = new List<OrdenResiduo>();

    [InverseProperty("IdResiduoNavigation")]
    public virtual ICollection<OrdenResiduo> OrdenResiduoIdResiduoNavigations { get; set; } = new List<OrdenResiduo>();

    [InverseProperty("IdResiduoNavigation")]
    public virtual ICollection<OrdenResiduoInsumo> OrdenResiduoInsumos { get; set; } = new List<OrdenResiduoInsumo>();

    [InverseProperty("IdResiduoNavigation")]
    public virtual ICollection<OrdenResiduoTransformacion> OrdenResiduoTransformacionIdResiduoNavigations { get; set; } = new List<OrdenResiduoTransformacion>();

    [InverseProperty("IdResiduoTransformacionNavigation")]
    public virtual ICollection<OrdenResiduoTransformacion> OrdenResiduoTransformacionIdResiduoTransformacionNavigations { get; set; } = new List<OrdenResiduoTransformacion>();

    [InverseProperty("IdResiduoNavigation")]
    public virtual ICollection<ResiduoTransformacion> ResiduoTransformacionIdResiduoNavigations { get; set; } = new List<ResiduoTransformacion>();

    [InverseProperty("IdResiduoTransformacionNavigation")]
    public virtual ICollection<ResiduoTransformacion> ResiduoTransformacionIdResiduoTransformacionNavigations { get; set; } = new List<ResiduoTransformacion>();

    [InverseProperty("IdResiduoNavigation")]
    public virtual ICollection<Saldo> Saldos { get; set; } = new List<Saldo>();

    [InverseProperty("IdResiduoNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdResiduoNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdResiduoOrigenNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdResiduoOrigenNavigations { get; set; } = new List<SolicitudDetalle>();
}
