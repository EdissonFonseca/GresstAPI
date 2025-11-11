using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdUsuario", "IdVehiculo")]
[Table("Usuario_Vehiculo")]
public partial class UsuarioVehiculo
{
    [Key]
    public long IdUsuario { get; set; }

    [Key]
    [StringLength(10)]
    public string IdVehiculo { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("UsuarioVehiculos")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [ForeignKey("IdVehiculo")]
    [InverseProperty("UsuarioVehiculos")]
    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
}
