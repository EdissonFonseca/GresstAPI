using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdRelacion", "IdVehiculo", "IdCuenta")]
[Table("Persona_Vehiculo")]
public partial class PersonaVehiculo
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    [StringLength(2)]
    public string IdRelacion { get; set; } = null!;

    [Key]
    [StringLength(10)]
    public string IdVehiculo { get; set; } = null!;

    [Key]
    public long IdCuenta { get; set; }

    public bool Activo { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaVehiculos")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdRelacion")]
    [InverseProperty("PersonaVehiculos")]
    public virtual Relacion IdRelacionNavigation { get; set; } = null!;

    [ForeignKey("IdVehiculo")]
    [InverseProperty("PersonaVehiculos")]
    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
}
