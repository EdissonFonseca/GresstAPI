using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCuenta", "TipoReferencia")]
[Table("Cuenta_Referencia")]
public partial class CuentaReferencium
{
    [Key]
    public long IdCuenta { get; set; }

    [Key]
    [StringLength(20)]
    public string TipoReferencia { get; set; } = null!;

    [StringLength(255)]
    public string? DescripcionReferencia { get; set; }

    public long? NumeroReferenciaInicio { get; set; }

    public long? NumeroReferenciaFin { get; set; }

    public long? NumeroReferenciaActual { get; set; }

    public bool? Autonumerar { get; set; }

    public bool? ReferenciaPorItem { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }
}
