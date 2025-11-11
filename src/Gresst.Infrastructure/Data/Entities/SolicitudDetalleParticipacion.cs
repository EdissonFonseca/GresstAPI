using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdSolicitud", "Item", "IdPersona", "IdDepositoOrigen")]
[Table("SolicitudDetalle_Participacion")]
public partial class SolicitudDetalleParticipacion
{
    [Key]
    public long IdSolicitud { get; set; }

    [Key]
    public int Item { get; set; }

    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdDepositoOrigen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Porcentaje { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDepositoOrigen")]
    [InverseProperty("SolicitudDetalleParticipacions")]
    public virtual Deposito IdDepositoOrigenNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("SolicitudDetalleParticipacions")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdSolicitud, Item")]
    [InverseProperty("SolicitudDetalleParticipacions")]
    public virtual SolicitudDetalle SolicitudDetalle { get; set; } = null!;
}
