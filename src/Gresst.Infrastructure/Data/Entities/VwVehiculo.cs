using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwVehiculo
{
    [StringLength(10)]
    public string IdVehiculo { get; set; } = null!;

    [StringLength(255)]
    public string? Descripcion { get; set; }

    [StringLength(268)]
    public string Nombre { get; set; } = null!;

    public int? Modelo { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Alto { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Ancho { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cubicaje { get; set; }

    [StringLength(10)]
    public string? IdCarroceria { get; set; }

    [StringLength(10)]
    public string? IdTipoVehiculo { get; set; }

    [StringLength(255)]
    public string? TipoVehiculo { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Largo { get; set; }

    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [StringLength(255)]
    public string? Carroceria { get; set; }

    public int Propio { get; set; }

    [StringLength(40)]
    public string? IdPropietario { get; set; }

    public long IdDeposito { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaRevision { get; set; }

    [Column("FechaSOAT", TypeName = "datetime")]
    public DateTime? FechaSoat { get; set; }

    public long IdCuenta { get; set; }
}
