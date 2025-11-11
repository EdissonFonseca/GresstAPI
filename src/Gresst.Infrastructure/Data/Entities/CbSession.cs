using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("CB_Sessions")]
public partial class CbSession
{
    [Key]
    [StringLength(25)]
    public string User { get; set; } = null!;

    [Column("Current_Step")]
    public int? CurrentStep { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Activity { get; set; }

    [InverseProperty("UserNavigation")]
    public virtual ICollection<CbStep> CbSteps { get; set; } = new List<CbStep>();
}
