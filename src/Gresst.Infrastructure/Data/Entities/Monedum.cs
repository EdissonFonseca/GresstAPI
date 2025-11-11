using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

public partial class Monedum
{
    [Key]
    public long IdMoneda { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [StringLength(3)]
    public string Sigla { get; set; } = null!;

    [StringLength(5)]
    public string Simbolo { get; set; } = null!;

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdMonedaNavigation")]
    public virtual ICollection<TarifaFacturacion> TarifaFacturacions { get; set; } = new List<TarifaFacturacion>();

    [InverseProperty("IdMonedaNavigation")]
    public virtual ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
}
