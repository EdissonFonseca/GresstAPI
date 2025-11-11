using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Embalaje")]
public partial class Embalaje
{
    [Key]
    public long IdEmbalaje { get; set; }

    public long? IdEmbalajeSuperior { get; set; }

    [StringLength(50)]
    public string? Nombre { get; set; }

    public bool Activo { get; set; }

    public bool Publico { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdEmbalajeSuperior")]
    [InverseProperty("InverseIdEmbalajeSuperiorNavigation")]
    public virtual Embalaje? IdEmbalajeSuperiorNavigation { get; set; }

    [ForeignKey("IdUsuarioCreacion")]
    [InverseProperty("EmbalajeIdUsuarioCreacionNavigations")]
    public virtual Usuario IdUsuarioCreacionNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioUltimaModificacion")]
    [InverseProperty("EmbalajeIdUsuarioUltimaModificacionNavigations")]
    public virtual Usuario? IdUsuarioUltimaModificacionNavigation { get; set; }

    [InverseProperty("IdEmbalajeSuperiorNavigation")]
    public virtual ICollection<Embalaje> InverseIdEmbalajeSuperiorNavigation { get; set; } = new List<Embalaje>();

    [InverseProperty("IdEmbalajeNavigation")]
    public virtual ICollection<PersonaEmbalaje> PersonaEmbalajes { get; set; } = new List<PersonaEmbalaje>();

    [InverseProperty("IdEmbalajeNavigation")]
    public virtual ICollection<PersonaMaterial> PersonaMaterials { get; set; } = new List<PersonaMaterial>();

    [InverseProperty("IdEmbalajeNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdEmbalajeNavigations { get; set; } = new List<SolicitudDetalle>();

    [InverseProperty("IdEmbalajeSolicitudNavigation")]
    public virtual ICollection<SolicitudDetalle> SolicitudDetalleIdEmbalajeSolicitudNavigations { get; set; } = new List<SolicitudDetalle>();
}
