using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdTarifa", "IdOpcion")]
[Table("Tarifa_Opcion")]
public partial class TarifaOpcion
{
    [Key]
    public long IdTarifa { get; set; }

    [Key]
    [StringLength(50)]
    public string IdOpcion { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdOpcion")]
    [InverseProperty("TarifaOpcions")]
    public virtual Opcion IdOpcionNavigation { get; set; } = null!;

    [ForeignKey("IdTarifa")]
    [InverseProperty("TarifaOpcions")]
    public virtual Tarifa IdTarifaNavigation { get; set; } = null!;
}
