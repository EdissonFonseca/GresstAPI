using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "TipoReferencia", "IdReferencia")]
public partial class Referencium
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    [StringLength(20)]
    public string TipoReferencia { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string IdReferencia { get; set; } = null!;

    public long? NumeroReferencia { get; set; }

    [StringLength(40)]
    public string? IdResponsable { get; set; }

    [StringLength(40)]
    public string? IdResponsable2 { get; set; }

    [StringLength(40)]
    public string? IdResponsable3 { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Fecha { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    public string? Notas { get; set; }

    public string? Soporte { get; set; }

    public string? DatosAdicionales { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("ReferenciumIdPersonaNavigations")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdResponsable2")]
    [InverseProperty("ReferenciumIdResponsable2Navigations")]
    public virtual Persona? IdResponsable2Navigation { get; set; }

    [ForeignKey("IdResponsable3")]
    [InverseProperty("ReferenciumIdResponsable3Navigations")]
    public virtual Persona? IdResponsable3Navigation { get; set; }

    [ForeignKey("IdResponsable")]
    [InverseProperty("ReferenciumIdResponsableNavigations")]
    public virtual Persona? IdResponsableNavigation { get; set; }

    [InverseProperty("Referencium")]
    public virtual ICollection<ReferenciaResiduo> ReferenciaResiduos { get; set; } = new List<ReferenciaResiduo>();
}
