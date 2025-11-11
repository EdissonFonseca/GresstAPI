using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Parametro")]
public partial class Parametro
{
    [Key]
    [StringLength(20)]
    public string IdParametro { get; set; } = null!;

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [StringLength(100)]
    public string? Valor { get; set; }

    public bool Oculto { get; set; }

    public bool Administrable { get; set; }

    [InverseProperty("IdParametroNavigation")]
    public virtual ICollection<CuentaParametro> CuentaParametros { get; set; } = new List<CuentaParametro>();
}
