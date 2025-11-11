using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwMaterialCodigoDescripcion
{
    public long IdMaterial { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    public string? Sinonimos { get; set; }

    public string? Descripcion { get; set; }

    [StringLength(1)]
    public string? Medicion { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? FactorCompensacionEmision { get; set; }

    public int? IdTipoResiduo { get; set; }

    [StringLength(255)]
    public string Tratamiento { get; set; } = null!;

    [StringLength(4000)]
    public string? CodigoA { get; set; }

    [StringLength(4000)]
    public string? DescripcionCodigoA { get; set; }

    [StringLength(4000)]
    public string? CodigoY { get; set; }

    [StringLength(4000)]
    public string? DescripcionCodigoY { get; set; }

    [StringLength(4000)]
    public string? CodigoPropio { get; set; }

    [StringLength(4000)]
    public string? Clasificacion { get; set; }

    [StringLength(4000)]
    public string? Caracteristicas { get; set; }

    public bool Publico { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioCompra { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioServicio { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }
}
