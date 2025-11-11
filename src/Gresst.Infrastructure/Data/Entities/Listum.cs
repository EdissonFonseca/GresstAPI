using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

public partial class Listum
{
    [Key]
    [Column("IdLIsta")]
    public long IdLista { get; set; }

    [StringLength(50)]
    public string? Nombre { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdListaNavigation")]
    public virtual ICollection<ListaItem> ListaItems { get; set; } = new List<ListaItem>();
}
