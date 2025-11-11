using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwPaisDepartamentoMunicipio
{
    [StringLength(10)]
    public string IdLocalizacion { get; set; } = null!;

    [StringLength(50)]
    public string? Nombre { get; set; }

    [StringLength(50)]
    public string? Pais { get; set; }

    [StringLength(50)]
    public string? Departamento { get; set; }

    [StringLength(50)]
    public string? Municipio { get; set; }

    [StringLength(50)]
    public string? Zona { get; set; }

    [StringLength(50)]
    public string? Sector { get; set; }
}
