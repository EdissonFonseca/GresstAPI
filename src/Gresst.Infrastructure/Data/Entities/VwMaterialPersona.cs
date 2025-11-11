using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwMaterialPersona
{
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    public long IdCuenta { get; set; }

    public long IdMaterial { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string? Sinonimos { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public int? IdTipoResiduo { get; set; }

    [StringLength(50)]
    public string? Imagen { get; set; }

    [StringLength(1)]
    public string? Medicion { get; set; }

    public bool Publico { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioServicio { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PrecioCompra { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? FactorCompensacionEmision { get; set; }

    public bool Activo { get; set; }

    public bool Aprovechable { get; set; }

    public long? IdEmbalaje { get; set; }

    [StringLength(255)]
    public string? TipoResiduo { get; set; }

    [StringLength(4000)]
    public string? CodigoA { get; set; }

    [StringLength(4000)]
    public string? CodigoY { get; set; }

    [StringLength(4000)]
    public string? CodigoPropio { get; set; }

    [StringLength(4000)]
    public string? Clasificacion { get; set; }

    [StringLength(4000)]
    public string? Caracteristicas { get; set; }

    [StringLength(4000)]
    public string? Tratamientos { get; set; }
}
