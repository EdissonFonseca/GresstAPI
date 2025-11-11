using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwDeposito
{
    public long IdCuenta { get; set; }

    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    public long IdDeposito { get; set; }

    [StringLength(255)]
    public string? Nombre { get; set; }

    [StringLength(255)]
    public string? Direccion { get; set; }

    public Geometry? Ubicacion { get; set; }

    [StringLength(50)]
    public string? Telefono { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public string? Notas { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    public bool Almacenamiento { get; set; }

    public bool Entrega { get; set; }

    public bool Recepcion { get; set; }

    public bool Disposicion { get; set; }

    public bool Tratamiento { get; set; }

    public bool Activo { get; set; }

    [StringLength(255)]
    public string? Correo { get; set; }

    public string? DatosAdicionales { get; set; }

    [StringLength(255)]
    public string? Sede { get; set; }

    [StringLength(50)]
    public string? ReferenciaSede { get; set; }

    [StringLength(255)]
    public string? DireccionSede { get; set; }

    public string? NotasSede { get; set; }

    [StringLength(50)]
    public string? TelefonoSede { get; set; }

    [StringLength(255)]
    public string? CorreoSede { get; set; }

    public string? DatosAdicionalesSede { get; set; }

    [StringLength(50)]
    public string? Localizacion { get; set; }

    [StringLength(10)]
    public string IdLocalizacion { get; set; } = null!;

    public int IdSede { get; set; }

    public long IdUsuario { get; set; }
}
