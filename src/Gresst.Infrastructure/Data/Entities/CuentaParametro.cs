using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCuenta", "IdParametro")]
[Table("Cuenta_Parametro")]
public partial class CuentaParametro
{
    [Key]
    public long IdCuenta { get; set; }

    [Key]
    [StringLength(20)]
    public string IdParametro { get; set; } = null!;

    [StringLength(100)]
    public string? Valor { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCuenta")]
    [InverseProperty("CuentaParametros")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    [ForeignKey("IdParametro")]
    [InverseProperty("CuentaParametros")]
    public virtual Parametro IdParametroNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioCreacion")]
    [InverseProperty("CuentaParametroIdUsuarioCreacionNavigations")]
    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioUltimaModificacion")]
    [InverseProperty("CuentaParametroIdUsuarioUltimaModificacionNavigations")]
    public virtual Usuario? IdUsuarioUltimaModificacionNavigation { get; set; }
}
