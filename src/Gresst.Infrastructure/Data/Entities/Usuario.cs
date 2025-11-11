using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data.Entities;

[Table("Usuario")]
[Index("Correo", Name = "Login", IsUnique = true)]
public partial class Usuario
{
    [Key]
    public long IdUsuario { get; set; }

    public long? IdUsuarioSuperior { get; set; }

    public long IdCuenta { get; set; }

    public long? IdCuentaOrigen { get; set; }

    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Apellido { get; set; }

    [StringLength(100)]
    public string Correo { get; set; } = null!;

    [StringLength(100)]
    public string? Clave { get; set; }

    [StringLength(1)]
    public string IdEstado { get; set; } = null!;

    [StringLength(40)]
    public string? IdPersona { get; set; }

    public string? DatosAdicionales { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Cuentum> Cuenta { get; set; } = new List<Cuentum>();

    [InverseProperty("IdUsuarioCreacionNavigation")]
    public virtual ICollection<CuentaParametro> CuentaParametroIdUsuarioCreacionNavigations { get; set; } = new List<CuentaParametro>();

    [InverseProperty("IdUsuarioUltimaModificacionNavigation")]
    public virtual ICollection<CuentaParametro> CuentaParametroIdUsuarioUltimaModificacionNavigations { get; set; } = new List<CuentaParametro>();

    [InverseProperty("IdUsuarioCreacionNavigation")]
    public virtual ICollection<Embalaje> EmbalajeIdUsuarioCreacionNavigations { get; set; } = new List<Embalaje>();

    [InverseProperty("IdUsuarioUltimaModificacionNavigation")]
    public virtual ICollection<Embalaje> EmbalajeIdUsuarioUltimaModificacionNavigations { get; set; } = new List<Embalaje>();

    [InverseProperty("IdUsuarioCreacionNavigation")]
    public virtual ICollection<Gestion> GestionIdUsuarioCreacionNavigations { get; set; } = new List<Gestion>();

    [InverseProperty("IdUsuarioUltimaModificacionNavigation")]
    public virtual ICollection<Gestion> GestionIdUsuarioUltimaModificacionNavigations { get; set; } = new List<Gestion>();

    [ForeignKey("IdCuenta")]
    [InverseProperty("UsuarioIdCuentaNavigations")]
    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;

    [ForeignKey("IdCuentaOrigen")]
    [InverseProperty("UsuarioIdCuentaOrigenNavigations")]
    public virtual Cuentum? IdCuentaOrigenNavigation { get; set; }

    [ForeignKey("IdPersona")]
    [InverseProperty("Usuarios")]
    public virtual Persona? IdPersonaNavigation { get; set; }

    [InverseProperty("IdUsuarioCreacionNavigation")]
    public virtual ICollection<OrdenInsumo> OrdenInsumoIdUsuarioCreacionNavigations { get; set; } = new List<OrdenInsumo>();

    [InverseProperty("IdUsuarioUltimaModificacionNavigation")]
    public virtual ICollection<OrdenInsumo> OrdenInsumoIdUsuarioUltimaModificacionNavigations { get; set; } = new List<OrdenInsumo>();

    [InverseProperty("IdUsuarioCreacionNavigation")]
    public virtual ICollection<OrdenResiduoInsumo> OrdenResiduoInsumoIdUsuarioCreacionNavigations { get; set; } = new List<OrdenResiduoInsumo>();

    [InverseProperty("IdUsuarioUltimaModificacionNavigation")]
    public virtual ICollection<OrdenResiduoInsumo> OrdenResiduoInsumoIdUsuarioUltimaModificacionNavigations { get; set; } = new List<OrdenResiduoInsumo>();

    [InverseProperty("IdUsuarioCreacionNavigation")]
    public virtual ICollection<Reporte> ReporteIdUsuarioCreacionNavigations { get; set; } = new List<Reporte>();

    [InverseProperty("IdUsuarioUltimaModificacionNavigation")]
    public virtual ICollection<Reporte> ReporteIdUsuarioUltimaModificacionNavigations { get; set; } = new List<Reporte>();

    [InverseProperty("IdUsuarioCreacionNavigation")]
    public virtual ICollection<SolicitudDetalleInsumo> SolicitudDetalleInsumos { get; set; } = new List<SolicitudDetalleInsumo>();

    [InverseProperty("IdUsuarioCreacionNavigation")]
    public virtual ICollection<SolicitudInsumo> SolicitudInsumoIdUsuarioCreacionNavigations { get; set; } = new List<SolicitudInsumo>();

    [InverseProperty("IdUsuarioUltimaModificacionNavigation")]
    public virtual ICollection<SolicitudInsumo> SolicitudInsumoIdUsuarioUltimaModificacionNavigations { get; set; } = new List<SolicitudInsumo>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<UsuarioDeposito> UsuarioDepositos { get; set; } = new List<UsuarioDeposito>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<UsuarioOpcion> UsuarioOpcions { get; set; } = new List<UsuarioOpcion>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<UsuarioPersona> UsuarioPersonas { get; set; } = new List<UsuarioPersona>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<UsuarioVehiculo> UsuarioVehiculos { get; set; } = new List<UsuarioVehiculo>();
}
