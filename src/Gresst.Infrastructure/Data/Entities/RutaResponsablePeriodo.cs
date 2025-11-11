using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdRuta", "IdPeriodo")]
[Table("Ruta_ResponsablePeriodo")]
public partial class RutaResponsablePeriodo
{
    [Key]
    public long IdRuta { get; set; }

    [Key]
    public int IdPeriodo { get; set; }

    [StringLength(40)]
    public string IdResponsable { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime FechaInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaFin { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdResponsable")]
    [InverseProperty("RutaResponsablePeriodos")]
    public virtual Persona IdResponsableNavigation { get; set; } = null!;

    [ForeignKey("IdRuta")]
    [InverseProperty("RutaResponsablePeriodos")]
    public virtual Rutum IdRutaNavigation { get; set; } = null!;
}
