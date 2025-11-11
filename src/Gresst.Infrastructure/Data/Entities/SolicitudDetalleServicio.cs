using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdSolicitud", "Item", "IdServicio")]
[Table("SolicitudDetalle_Servicio")]
public partial class SolicitudDetalleServicio
{
    [Key]
    public long IdSolicitud { get; set; }

    [Key]
    public int Item { get; set; }

    [Key]
    public long IdServicio { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdSolicitud")]
    [InverseProperty("SolicitudDetalleServicios")]
    public virtual Solicitud IdSolicitudNavigation { get; set; } = null!;

    [ForeignKey("IdSolicitud, Item")]
    [InverseProperty("SolicitudDetalleServicios")]
    public virtual SolicitudDetalle SolicitudDetalle { get; set; } = null!;
}
