using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdResiduo", "IdResiduoTransformacion")]
[Table("ResiduoTransformacion")]
public partial class ResiduoTransformacion
{
    [Key]
    public long IdResiduo { get; set; }

    [Key]
    public long IdResiduoTransformacion { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdResiduo")]
    [InverseProperty("ResiduoTransformacionIdResiduoNavigations")]
    public virtual Residuo IdResiduoNavigation { get; set; } = null!;

    [ForeignKey("IdResiduoTransformacion")]
    [InverseProperty("ResiduoTransformacionIdResiduoTransformacionNavigations")]
    public virtual Residuo IdResiduoTransformacionNavigation { get; set; } = null!;
}
