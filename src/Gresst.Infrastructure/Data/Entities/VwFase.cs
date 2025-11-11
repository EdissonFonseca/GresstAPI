using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwFase
{
    [StringLength(5)]
    public string Id { get; set; } = null!;

    [StringLength(50)]
    public string Nombre { get; set; } = null!;
}
