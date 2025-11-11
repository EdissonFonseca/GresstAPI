using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Keyless]
public partial class VwSolicitudParticion
{
    public long IdSolicitud { get; set; }

    public long? NumeroSolicitud { get; set; }

    [StringLength(40)]
    public string? IdSolicitante { get; set; }

    public long? IdDepositoOrigen { get; set; }

    [StringLength(40)]
    public string? IdTransportador { get; set; }

    [StringLength(10)]
    public string? IdVehiculo { get; set; }

    [StringLength(40)]
    public string? IdProveedor { get; set; }

    public long? IdDepositoDestino { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaSolicitud { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaRecepcion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaProceso { get; set; }
}
