using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwInformacionMaterial
{
    public long IdMaterial { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    public int? IdTipoResiduo { get; set; }

    [StringLength(255)]
    public string TipoResiduo { get; set; } = null!;

    [StringLength(100)]
    public string Tratamiento { get; set; } = null!;
}
