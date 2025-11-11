using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Mensaje")]
[Index("IdPersonaReceptor", Name = "Idx_Mensaje_Receptor")]
public partial class Mensaje
{
    [Key]
    public long IdMensaje { get; set; }

    [StringLength(40)]
    public string IdPersonaEmisor { get; set; } = null!;

    [StringLength(40)]
    public string? IdPersonaReceptor { get; set; }

    public long? IdEmisor { get; set; }

    [StringLength(1)]
    public string IdTipo { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Fecha { get; set; }

    [StringLength(255)]
    public string? Titulo { get; set; }

    [Column("Mensaje")]
    public string? Mensaje1 { get; set; }

    public long? IdSolicitud { get; set; }

    public long? IdOrden { get; set; }

    public long? IdCertificado { get; set; }

    [StringLength(255)]
    public string? Adjunto { get; set; }

    [StringLength(255)]
    public string? MensajeReferencia { get; set; }

    /// <summary>
    /// JSON Con arreglo de usuarios, marcas de lectura y otras marcas que cada receptor coloca al mensaje
    /// </summary>
    public string? Receptores { get; set; }

    public bool? Leido { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCertificado")]
    [InverseProperty("Mensajes")]
    public virtual Certificado? IdCertificadoNavigation { get; set; }

    [ForeignKey("IdOrden")]
    [InverseProperty("Mensajes")]
    public virtual Orden? IdOrdenNavigation { get; set; }

    [ForeignKey("IdPersonaEmisor")]
    [InverseProperty("MensajeIdPersonaEmisorNavigations")]
    public virtual Persona IdPersonaEmisorNavigation { get; set; } = null!;

    [ForeignKey("IdPersonaReceptor")]
    [InverseProperty("MensajeIdPersonaReceptorNavigations")]
    public virtual Persona? IdPersonaReceptorNavigation { get; set; }

    [ForeignKey("IdSolicitud")]
    [InverseProperty("Mensajes")]
    public virtual Solicitud? IdSolicitudNavigation { get; set; }
}
