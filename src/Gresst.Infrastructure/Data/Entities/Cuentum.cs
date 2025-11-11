using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

public partial class Cuentum
{
    [Key]
    public long IdCuenta { get; set; }

    /// <summary>
    /// Usuario propietario de la cuenta
    /// </summary>
    public long IdUsuario { get; set; }

    [StringLength(40)]
    public string IdPersona { get; set; } = null!;

    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    [StringLength(1)]
    public string IdRol { get; set; } = null!;

    [StringLength(1)]
    public string? IdEstado { get; set; }

    public string? Ajustes { get; set; }

    public bool PermisosPorSede { get; set; }

    public long IdUsuarioCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    public long? IdUsuarioUltimaModificacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<CuentaCertificado> CuentaCertificados { get; set; } = new List<CuentaCertificado>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<CuentaInterfaz> CuentaInterfazs { get; set; } = new List<CuentaInterfaz>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<CuentaOpcion> CuentaOpcions { get; set; } = new List<CuentaOpcion>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<CuentaParametro> CuentaParametros { get; set; } = new List<CuentaParametro>();

    [ForeignKey("IdPersona")]
    [InverseProperty("Cuenta")]
    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("Cuenta")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<PersonaContacto> PersonaContactos { get; set; } = new List<PersonaContacto>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<PersonaEmbalaje> PersonaEmbalajes { get; set; } = new List<PersonaEmbalaje>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<PersonaInsumo> PersonaInsumos { get; set; } = new List<PersonaInsumo>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<PersonaLicencium> PersonaLicencia { get; set; } = new List<PersonaLicencium>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<Rutum> Ruta { get; set; } = new List<Rutum>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<TarifaCuentum> TarifaCuenta { get; set; } = new List<TarifaCuentum>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<TarifaFacturacion> TarifaFacturacions { get; set; } = new List<TarifaFacturacion>();

    [InverseProperty("IdCuentaNavigation")]
    public virtual ICollection<Usuario> UsuarioIdCuentaNavigations { get; set; } = new List<Usuario>();

    [InverseProperty("IdCuentaOrigenNavigation")]
    public virtual ICollection<Usuario> UsuarioIdCuentaOrigenNavigations { get; set; } = new List<Usuario>();
}
