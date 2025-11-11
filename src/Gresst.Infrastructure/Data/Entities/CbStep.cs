using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("CB_Steps")]
public partial class CbStep
{
    [Key]
    public int Id { get; set; }

    [StringLength(25)]
    public string? User { get; set; }

    public int? Step { get; set; }

    [StringLength(250)]
    public string? Value { get; set; }

    [ForeignKey("User")]
    [InverseProperty("CbSteps")]
    public virtual CbSession? UserNavigation { get; set; }
}
