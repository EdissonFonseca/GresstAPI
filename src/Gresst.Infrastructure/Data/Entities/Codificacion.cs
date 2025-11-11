using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("Id", "IdCategoria")]
[Table("Codificacion")]
public partial class Codificacion
{
    [Key]
    [StringLength(20)]
    public string IdCategoria { get; set; } = null!;

    [Key]
    [StringLength(20)]
    public string Id { get; set; } = null!;

    [StringLength(20)]
    public string? IdSuperior { get; set; }

    [StringLength(500)]
    public string? Descripcion { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("Codificacion")]
    public virtual ICollection<TipoResiduoHomologacion> TipoResiduoHomologacions { get; set; } = new List<TipoResiduoHomologacion>();
}
