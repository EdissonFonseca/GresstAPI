using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwSede
{
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    public int IdUbicacion { get; set; }

    [StringLength(10)]
    public string IdLocalizacion { get; set; } = null!;

    [StringLength(255)]
    public string? Nombre { get; set; }

    [StringLength(255)]
    public string? Direccion { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    public string? Notas { get; set; }

    public bool Activo { get; set; }

    [StringLength(255)]
    public string? Correo { get; set; }

    public string? DatosAdicionales { get; set; }

    [StringLength(50)]
    public string? Localizacion { get; set; }
}
