using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdRuta", "IdDeposito")]
[Table("Ruta_Deposito")]
public partial class RutaDeposito
{
    [Key]
    public long IdRuta { get; set; }

    [Key]
    public long IdDeposito { get; set; }

    public int Orden { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDeposito")]
    [InverseProperty("RutaDepositos")]
    public virtual Deposito IdDepositoNavigation { get; set; } = null!;

    [ForeignKey("IdRuta")]
    [InverseProperty("RutaDepositos")]
    public virtual Rutum IdRutaNavigation { get; set; } = null!;
}
