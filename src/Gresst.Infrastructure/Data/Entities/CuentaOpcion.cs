using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCuenta", "IdOpcion")]
[Table("Cuenta_Opcion")]
public partial class CuentaOpcion
{
    [Key]
    public long IdCuenta { get; set; }

    [Key]
    [StringLength(50)]
    public string IdOpcion { get; set; } = null!;

    [StringLength(500)]
    public string? Settings { get; set; }

    public bool EnviarCorreo { get; set; }

    public string? DestinatariosCorreo { get; set; }

    [StringLength(4)]
    public string? Permisos { get; set; }

    public long IdCuentaCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdCuentaUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCuenta")]
    [InverseProperty("CuentaOpcions")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    [ForeignKey("IdOpcion")]
    [InverseProperty("CuentaOpcions")]
    public virtual Opcion IdOpcionNavigation { get; set; } = null!;
}
