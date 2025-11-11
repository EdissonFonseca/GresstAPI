using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

public partial class Rutum
{
    [Key]
    public long IdRuta { get; set; }

    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    public long IdCuenta { get; set; }

    [StringLength(10)]
    public string IdVehiculo { get; set; } = null!;

    [StringLength(40)]
    public string IdResponsable { get; set; } = null!;

    public string Recurrencia { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? FechaInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaFin { get; set; }

    public bool DiaCompleto { get; set; }

    public TimeOnly? HoraInicio { get; set; }

    public TimeOnly? HoraFin { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Duracion { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCuenta")]
    [InverseProperty("Ruta")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    [ForeignKey("IdResponsable")]
    [InverseProperty("Ruta")]
    public virtual Persona IdResponsableNavigation { get; set; } = null!;

    [ForeignKey("IdVehiculo")]
    [InverseProperty("Ruta")]
    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;

    [InverseProperty("IdRutaNavigation")]
    public virtual ICollection<RutaDeposito> RutaDepositos { get; set; } = new List<RutaDeposito>();

    [InverseProperty("IdRutaNavigation")]
    public virtual ICollection<RutaResponsablePeriodo> RutaResponsablePeriodos { get; set; } = new List<RutaResponsablePeriodo>();
}
