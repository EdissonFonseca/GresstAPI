using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[PrimaryKey("IdPersona", "IdLicencia", "IdCuenta")]
[Table("Persona_Licencia")]
public partial class PersonaLicencium
{
    [Key]
    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [Key]
    public long IdLicencia { get; set; }

    [Key]
    public long IdCuenta { get; set; }

    public long? IdDeposito { get; set; }

    public long? IdTratamiento { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [ForeignKey("IdCuenta")]
    [InverseProperty("PersonaLicencia")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    [ForeignKey("IdDeposito")]
    [InverseProperty("PersonaLicencia")]
    public virtual Deposito? IdDepositoNavigation { get; set; }

    [ForeignKey("IdLicencia")]
    [InverseProperty("PersonaLicencia")]
    public virtual Licencium IdLicenciaNavigation { get; set; } = null!;

    [ForeignKey("IdPersona")]
    [InverseProperty("PersonaLicencia")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
