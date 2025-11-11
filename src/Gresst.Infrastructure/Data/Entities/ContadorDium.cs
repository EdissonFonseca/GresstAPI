using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

public partial class ContadorDium
{
    [Key]
    public DateOnly Fecha { get; set; }

    public int NumeroCorreosEnviados { get; set; }
}
