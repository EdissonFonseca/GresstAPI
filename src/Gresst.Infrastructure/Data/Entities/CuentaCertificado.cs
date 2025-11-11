using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCuenta", "IdCertificado")]
[Table("Cuenta_Certificado")]
public partial class CuentaCertificado
{
    [Key]
    public long IdCuenta { get; set; }

    [Key]
    public long IdCertificado { get; set; }

    [StringLength(10)]
    public string? NumeroFactura { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCertificado")]
    [InverseProperty("CuentaCertificados")]
    public virtual Certificado IdCertificadoNavigation { get; set; } = null!;

    [ForeignKey("IdCuenta")]
    [InverseProperty("CuentaCertificados")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;
}
