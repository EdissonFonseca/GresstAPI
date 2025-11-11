using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCertificado", "IdPersona")]
[Table("Certificado_Persona")]
public partial class CertificadoPersona
{
    [Key]
    public long IdCertificado { get; set; }

    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    public bool Publicado { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCertificado")]
    [InverseProperty("CertificadoPersonas")]
    public virtual Certificado IdCertificadoNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("CertificadoPersonas")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
