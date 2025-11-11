using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("TipoResiduo")]
public partial class TipoResiduo
{
    [Key]
    public int IdTipoResiduo { get; set; }

    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [StringLength(50)]
    public string? Imagen { get; set; }

    public bool Publico { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<DepositoTipoResiduo> DepositoTipoResiduos { get; set; } = new List<DepositoTipoResiduo>();

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<PersonaTipoResiduo> PersonaTipoResiduos { get; set; } = new List<PersonaTipoResiduo>();

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<TipoResiduoCaracteristica> TipoResiduoCaracteristicas { get; set; } = new List<TipoResiduoCaracteristica>();

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<TipoResiduoClasificacion> TipoResiduoClasificacions { get; set; } = new List<TipoResiduoClasificacion>();

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<TipoResiduoFrase> TipoResiduoFrases { get; set; } = new List<TipoResiduoFrase>();

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<TipoResiduoHomologacion> TipoResiduoHomologacions { get; set; } = new List<TipoResiduoHomologacion>();

    [InverseProperty("IdItemNavigation")]
    public virtual ICollection<TipoResiduoItem> TipoResiduoItemIdItemNavigations { get; set; } = new List<TipoResiduoItem>();

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<TipoResiduoItem> TipoResiduoItemIdTipoResiduoNavigations { get; set; } = new List<TipoResiduoItem>();

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<TipoResiduoPictograma> TipoResiduoPictogramas { get; set; } = new List<TipoResiduoPictograma>();

    [InverseProperty("IdTipoResiduoNavigation")]
    public virtual ICollection<TipoResiduoTratamiento> TipoResiduoTratamientos { get; set; } = new List<TipoResiduoTratamiento>();
}
