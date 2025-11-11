using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Reporte")]
public partial class Reporte
{
    [Key]
    public long IdReporte { get; set; }

    [StringLength(40)]
    public string? IdPersona { get; set; }

    [StringLength(255)]
    public string? Descripcion { get; set; }

    [StringLength(500)]
    public string? Comando { get; set; }

    public byte[]? Layout { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("Reportes")]
    public virtual Persona? IdPersonaNavigation { get; set; }

    [ForeignKey("IdUsuarioCreacion")]
    [InverseProperty("ReporteIdUsuarioCreacionNavigations")]
    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioUltimaModificacion")]
    [InverseProperty("ReporteIdUsuarioUltimaModificacionNavigations")]
    public virtual Usuario? IdUsuarioUltimaModificacionNavigation { get; set; }
}
