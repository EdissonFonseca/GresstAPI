using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

public partial class Licencium
{
    [Key]
    public long IdLicencia { get; set; }

    [StringLength(50)]
    public string? Numero { get; set; }

    [StringLength(50)]
    public string? Descripcion { get; set; }

    public string Texto { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime FechaInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaFin { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdLicenciaNavigation")]
    public virtual ICollection<CertificadoLicencium> CertificadoLicencia { get; set; } = new List<CertificadoLicencium>();

    [InverseProperty("IdLicenciaNavigation")]
    public virtual ICollection<PersonaLicencium> PersonaLicencia { get; set; } = new List<PersonaLicencium>();
}
