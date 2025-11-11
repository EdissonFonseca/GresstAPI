using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCuenta", "Interfaz")]
[Table("Cuenta_Interfaz")]
public partial class CuentaInterfaz
{
    [Key]
    public long IdCuenta { get; set; }

    [Key]
    [StringLength(50)]
    public string Interfaz { get; set; } = null!;

    [StringLength(100)]
    public string? Conexion { get; set; }

    [StringLength(100)]
    public string? Url { get; set; }

    [StringLength(50)]
    public string? Usuario { get; set; }

    [StringLength(10)]
    public string? Clave { get; set; }

    [StringLength(100)]
    public string? Llave { get; set; }

    [StringLength(1000)]
    public string? Token { get; set; }

    public string? Configuracion { get; set; }

    public string? Homologaciones { get; set; }

    [ForeignKey("IdCuenta")]
    [InverseProperty("CuentaInterfazs")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;
}
