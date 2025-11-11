using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Opcion")]
public partial class Opcion
{
    [Key]
    [StringLength(50)]
    public string IdOpcion { get; set; } = null!;

    [StringLength(50)]
    public string? IdOpcionSuperior { get; set; }

    [StringLength(50)]
    public string? Descripcion { get; set; }

    [StringLength(500)]
    public string? Settings { get; set; }

    public bool Configurable { get; set; }

    [StringLength(1)]
    public string IdRol { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdOpcionNavigation")]
    public virtual ICollection<CuentaOpcion> CuentaOpcions { get; set; } = new List<CuentaOpcion>();

    [ForeignKey("IdOpcionSuperior")]
    [InverseProperty("InverseIdOpcionSuperiorNavigation")]
    public virtual Opcion? IdOpcionSuperiorNavigation { get; set; }

    [InverseProperty("IdOpcionSuperiorNavigation")]
    public virtual ICollection<Opcion> InverseIdOpcionSuperiorNavigation { get; set; } = new List<Opcion>();

    [InverseProperty("IdOpcionNavigation")]
    public virtual ICollection<TarifaOpcion> TarifaOpcions { get; set; } = new List<TarifaOpcion>();

    [InverseProperty("IdOpcionNavigation")]
    public virtual ICollection<UsuarioOpcion> UsuarioOpcions { get; set; } = new List<UsuarioOpcion>();
}
