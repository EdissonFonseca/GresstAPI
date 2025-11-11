using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwCertificadoSolicitud
{
    public long IdCertificado { get; set; }

    public long? NumeroCertificado { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaExpedicion { get; set; }

    [StringLength(40)]
    public string? IdGenerador { get; set; }

    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [StringLength(40)]
    public string? IdResponsable { get; set; }

    public long IdSolicitud { get; set; }

    [StringLength(40)]
    public string? IdPropietario { get; set; }

    [StringLength(40)]
    public string? IdSolicitante { get; set; }

    public long? IdDepositoOrigen { get; set; }

    public long? IdDepositoDestino { get; set; }
}
