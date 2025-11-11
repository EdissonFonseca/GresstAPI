using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Certificado")]
public partial class Certificado
{
    /// <summary>
    /// &apos;T&apos;ransporte,&apos;D&apos;isposición,&apos;A&apos;lmacenamiento,&apos;R&apos;ecepción,&apos;M&apos; Tratamiento, &apos;N&apos; Transferencia
    /// </summary>
    [Key]
    public long IdCertificado { get; set; }

    public long NumeroCertificado { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    [StringLength(1)]
    public string IdTipo { get; set; } = null!;

    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Fecha { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    [StringLength(40)]
    public string? IdResponsable { get; set; }

    [StringLength(40)]
    public string? IdSolicitante { get; set; }

    [StringLength(40)]
    public string? IdGenerador { get; set; }

    public long? IdSolicitud { get; set; }

    public long? IdOrden { get; set; }

    public long? IdDepositoOrigen { get; set; }

    public string? Notas { get; set; }

    public string? Soporte { get; set; }

    public bool? Acumulado { get; set; }

    public bool? Agrupado { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdCertificadoNavigation")]
    public virtual ICollection<CertificadoLicencium> CertificadoLicencia { get; set; } = new List<CertificadoLicencium>();

    [InverseProperty("IdCertificadoNavigation")]
    public virtual ICollection<CertificadoPersona> CertificadoPersonas { get; set; } = new List<CertificadoPersona>();

    [InverseProperty("IdCertificadoNavigation")]
    public virtual ICollection<CertificadoResiduo> CertificadoResiduos { get; set; } = new List<CertificadoResiduo>();

    [InverseProperty("IdCertificadoNavigation")]
    public virtual ICollection<CuentaCertificado> CuentaCertificados { get; set; } = new List<CuentaCertificado>();

    [InverseProperty("IdCertificadoNavigation")]
    public virtual ICollection<Gestion> Gestions { get; set; } = new List<Gestion>();

    [ForeignKey("IdDepositoOrigen")]
    [InverseProperty("Certificados")]
    public virtual Deposito? IdDepositoOrigenNavigation { get; set; }

    [ForeignKey("IdGenerador")]
    [InverseProperty("CertificadoIdGeneradorNavigations")]
    public virtual Persona? IdGeneradorNavigation { get; set; }

    [ForeignKey("IdOrden")]
    [InverseProperty("Certificados")]
    public virtual Orden? IdOrdenNavigation { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("CertificadoIdPersonaNavigations")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdResponsable")]
    [InverseProperty("CertificadoIdResponsableNavigations")]
    public virtual Persona? IdResponsableNavigation { get; set; }

    [ForeignKey("IdSolicitante")]
    [InverseProperty("CertificadoIdSolicitanteNavigations")]
    public virtual Persona? IdSolicitanteNavigation { get; set; }

    [ForeignKey("IdSolicitud")]
    [InverseProperty("Certificados")]
    public virtual Solicitud? IdSolicitudNavigation { get; set; }

    [InverseProperty("IdCertificadoNavigation")]
    public virtual ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();

    [InverseProperty("IdCertificadoNavigation")]
    public virtual ICollection<OrdenResiduo> OrdenResiduos { get; set; } = new List<OrdenResiduo>();
}
