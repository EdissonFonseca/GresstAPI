using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("TipoReferencia", "IdReferencia", "IdPersona", "IdResiduo")]
[Table("ReferenciaResiduo")]
public partial class ReferenciaResiduo
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

    [Key]
    public long IdResiduo { get; set; }

    public string? Soporte { get; set; }

    public string? Notas { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPersona, TipoReferencia, IdReferencia")]
    [InverseProperty("ReferenciaResiduos")]
    public virtual Referencium Referencium { get; set; } = null!;
}
