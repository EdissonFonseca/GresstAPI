using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdOrden", "IdResiduo", "IdMaterialTransformacion")]
[Table("OrdenResiduoTransformacion")]
public partial class OrdenResiduoTransformacion
{
    [Key]
    public long IdOrden { get; set; }

    [Key]
    public long IdResiduo { get; set; }

    [Key]
    public long IdMaterialTransformacion { get; set; }

    public long? IdResiduoTransformacion { get; set; }

    [StringLength(40)]
    public string? IdPropietario { get; set; }

    public long? IdTratamiento { get; set; }

    public long? IdDepositoDestino { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Porcentaje { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? CantidadOrden { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? PesoOrden { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? VolumenOrden { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Peso { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Volumen { get; set; }

    [Column(TypeName = "numeric(18, 2)")]
    public decimal? Cantidad { get; set; }

    public string? Soporte { get; set; }

    [StringLength(50)]
    public string? Referencia { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdDepositoDestino")]
    [InverseProperty("OrdenResiduoTransformacions")]
    public virtual Deposito? IdDepositoDestinoNavigation { get; set; }

    [ForeignKey("IdOrden")]
    [InverseProperty("OrdenResiduoTransformacions")]
    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    [ForeignKey("IdPropietario")]
    [InverseProperty("OrdenResiduoTransformacions")]
    public virtual Persona? IdPropietarioNavigation { get; set; }

    [ForeignKey("IdResiduo")]
    [InverseProperty("OrdenResiduoTransformacionIdResiduoNavigations")]
    public virtual Residuo IdResiduoNavigation { get; set; } = null!;

    [ForeignKey("IdResiduoTransformacion")]
    [InverseProperty("OrdenResiduoTransformacionIdResiduoTransformacionNavigations")]
    public virtual Residuo? IdResiduoTransformacionNavigation { get; set; }

    [ForeignKey("IdTratamiento")]
    [InverseProperty("OrdenResiduoTransformacions")]
    public virtual Tratamiento? IdTratamientoNavigation { get; set; }

    [ForeignKey("IdOrden, IdResiduo")]
    [InverseProperty("OrdenResiduoTransformacions")]
    public virtual OrdenResiduo OrdenResiduo { get; set; } = null!;
}
