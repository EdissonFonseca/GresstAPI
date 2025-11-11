using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCertificado", "IdResiduo")]
[Table("CertificadoResiduo")]
[Index("IdResiduo", Name = "IdxCertificadoResiduoResiduo")]
public partial class CertificadoResiduo
{
    [Key]
    public long IdCertificado { get; set; }

    [Key]
    public long IdResiduo { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    public string? Soporte { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCertificado")]
    [InverseProperty("CertificadoResiduos")]
    public virtual Certificado IdCertificadoNavigation { get; set; } = null!;

    [ForeignKey("IdResiduo")]
    [InverseProperty("CertificadoResiduos")]
    public virtual Residuo IdResiduoNavigation { get; set; } = null!;
}
