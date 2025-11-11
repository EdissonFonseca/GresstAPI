using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("Elemento", "Id")]
[Table("NombreElemento")]
public partial class NombreElemento
{
    [Key]
    [StringLength(20)]
    public string Elemento { get; set; } = null!;

    [Key]
    [StringLength(5)]
    public string Id { get; set; } = null!;

    [StringLength(50)]
    public string Nombre { get; set; } = null!;
}
