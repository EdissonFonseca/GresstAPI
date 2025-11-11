using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCertificado", "IdLicencia")]
[Table("Certificado_Licencia")]
public partial class CertificadoLicencium
{
    [Key]
    public long IdCertificado { get; set; }

    [Key]
    public long IdLicencia { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCertificado")]
    [InverseProperty("CertificadoLicencia")]
    public virtual Certificado IdCertificadoNavigation { get; set; } = null!;

    [ForeignKey("IdLicencia")]
    [InverseProperty("CertificadoLicencia")]
    public virtual Licencium IdLicenciaNavigation { get; set; } = null!;
}
