using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwTipoResiduo
{
    public int IdTipoResiduo { get; set; }

    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [StringLength(4000)]
    public string? CodigoA { get; set; }

    [StringLength(4000)]
    public string? CodigoY { get; set; }

    [Column("CodigoUN")]
    [StringLength(4000)]
    public string? CodigoUn { get; set; }

    [Column("CodigoLER")]
    [StringLength(4000)]
    public string? CodigoLer { get; set; }

    [StringLength(4000)]
    public string? CodigoPropio { get; set; }

    [StringLength(4000)]
    public string? Clasificaciones { get; set; }

    [StringLength(4000)]
    public string? Caracteristicas { get; set; }

    public string? Tratamientos { get; set; }

    public string? Clientes { get; set; }

    public bool Publico { get; set; }
}
