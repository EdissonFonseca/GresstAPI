using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdDeposito", "Fecha")]
[Table("PlaneacionResponsable")]
public partial class PlaneacionResponsable
{
    [Key]
    public long IdDeposito { get; set; }

    [Key]
    [Column(TypeName = "datetime")]
    public DateTime Fecha { get; set; }

    [StringLength(40)]
    public string IdResponsable { get; set; } = null!;

    [StringLength(10)]
    public string? IdVehiculo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("PlaneacionResponsables")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdResponsable")]
    [InverseProperty("PlaneacionResponsables")]
    public virtual Persona IdResponsableNavigation { get; set; } = null!;

    [ForeignKey("IdVehiculo")]
    [InverseProperty("PlaneacionResponsables")]
    public virtual Vehiculo? IdVehiculoNavigation { get; set; }
}
