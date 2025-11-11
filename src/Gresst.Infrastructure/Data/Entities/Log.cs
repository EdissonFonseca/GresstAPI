using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Log")]
public partial class Log
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    public string MachineName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Logged { get; set; }

    [StringLength(50)]
    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;

    [StringLength(250)]
    public string? Logger { get; set; }

    public string? Callsite { get; set; }

    public string? Exception { get; set; }

    public string? Properties { get; set; }

    [Column("IP")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [StringLength(255)]
    public string? UserName { get; set; }

    public long? UserId { get; set; }
}
