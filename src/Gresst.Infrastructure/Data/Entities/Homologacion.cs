using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdCuenta", "IdPersona")]
[Table("Homologacion")]
public partial class Homologacion
{
    [Key]
    public long IdCuenta { get; set; }

    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [StringLength(40)]
    public string? IdNuevo { get; set; }
}
