using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdDeposito", "IdRelacion", "IdVehiculo")]
[Table("Deposito_Vehiculo")]
public partial class DepositoVehiculo
{
    [Key]
    public long IdDeposito { get; set; }

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    [StringLength(10)]
    public string IdVehiculo { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("DepositoVehiculos")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("DepositoVehiculos")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;

    [ForeignKey("IdVehiculo")]
    [InverseProperty("DepositoVehiculos")]
    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
}
