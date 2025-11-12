using System;
using System.Collections.Generic;
using Gresst.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Data;

public partial class InfrastructureDbContext : DbContext
{
    public InfrastructureDbContext(DbContextOptions<InfrastructureDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ajuste> Ajustes { get; set; }
    
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Caracteristica> Caracteristicas { get; set; }

    public virtual DbSet<Categorium> Categoria { get; set; }

    public virtual DbSet<CbSession> CbSessions { get; set; }

    public virtual DbSet<CbStep> CbSteps { get; set; }

    public virtual DbSet<Certificado> Certificados { get; set; }

    public virtual DbSet<CertificadoLicencium> CertificadoLicencia { get; set; }

    public virtual DbSet<CertificadoPersona> CertificadoPersonas { get; set; }

    public virtual DbSet<CertificadoResiduo> CertificadoResiduos { get; set; }

    public virtual DbSet<Clasificacion> Clasificacions { get; set; }

    public virtual DbSet<ClasificacionItem> ClasificacionItems { get; set; }

    public virtual DbSet<Codificacion> Codificacions { get; set; }

    public virtual DbSet<ContadorDium> ContadorDia { get; set; }

    public virtual DbSet<CuentaCertificado> CuentaCertificados { get; set; }

    public virtual DbSet<CuentaInterfaz> CuentaInterfazs { get; set; }

    public virtual DbSet<CuentaOpcion> CuentaOpcions { get; set; }

    public virtual DbSet<CuentaParametro> CuentaParametros { get; set; }

    public virtual DbSet<CuentaReferencium> CuentaReferencia { get; set; }

    public virtual DbSet<Cuentum> Cuenta { get; set; }

    public virtual DbSet<Deposito> Depositos { get; set; }

    public virtual DbSet<DepositoContacto> DepositoContactos { get; set; }

    public virtual DbSet<DepositoLocalizacion> DepositoLocalizacions { get; set; }

    public virtual DbSet<DepositoTipoResiduo> DepositoTipoResiduos { get; set; }

    public virtual DbSet<DepositoVehiculo> DepositoVehiculos { get; set; }

    public virtual DbSet<Embalaje> Embalajes { get; set; }

    public virtual DbSet<Frase> Frases { get; set; }

    public virtual DbSet<Gestion> Gestions { get; set; }

    public virtual DbSet<Homologacion> Homologacions { get; set; }

    public virtual DbSet<Insumo> Insumos { get; set; }

    public virtual DbSet<Licencium> Licencia { get; set; }

    public virtual DbSet<ListaItem> ListaItems { get; set; }

    public virtual DbSet<Listum> Lista { get; set; }

    public virtual DbSet<Localizacion> Localizacions { get; set; }

    public virtual DbSet<LocalizacionItem> LocalizacionItems { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<MaterialItem> MaterialItems { get; set; }

    public virtual DbSet<Mensaje> Mensajes { get; set; }

    public virtual DbSet<Monedum> Moneda { get; set; }

    public virtual DbSet<NombreElemento> NombreElementos { get; set; }

    public virtual DbSet<Opcion> Opcions { get; set; }

    public virtual DbSet<Orden> Ordens { get; set; }

    public virtual DbSet<OrdenInsumo> OrdenInsumos { get; set; }

    public virtual DbSet<OrdenPlaneacion> OrdenPlaneacions { get; set; }

    public virtual DbSet<OrdenResiduo> OrdenResiduos { get; set; }

    public virtual DbSet<OrdenResiduoInsumo> OrdenResiduoInsumos { get; set; }

    public virtual DbSet<OrdenResiduoTransformacion> OrdenResiduoTransformacions { get; set; }

    public virtual DbSet<Parametro> Parametros { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<PersonaContacto> PersonaContactos { get; set; }

    public virtual DbSet<PersonaEmbalaje> PersonaEmbalajes { get; set; }

    public virtual DbSet<PersonaInsumo> PersonaInsumos { get; set; }

    public virtual DbSet<PersonaLicencium> PersonaLicencia { get; set; }

    public virtual DbSet<PersonaLocalizacion> PersonaLocalizacions { get; set; }

    public virtual DbSet<PersonaLocalizacionArea> PersonaLocalizacionAreas { get; set; }

    public virtual DbSet<PersonaLocalizacionContacto> PersonaLocalizacionContactos { get; set; }

    public virtual DbSet<PersonaLocalizacionDeposito> PersonaLocalizacionDepositos { get; set; }

    public virtual DbSet<PersonaMaterial> PersonaMaterials { get; set; }

    public virtual DbSet<PersonaMaterialDeposito> PersonaMaterialDepositos { get; set; }

    public virtual DbSet<PersonaMaterialDepositoPrecio> PersonaMaterialDepositoPrecios { get; set; }

    public virtual DbSet<PersonaMaterialItem> PersonaMaterialItems { get; set; }

    public virtual DbSet<PersonaMaterialPrecio> PersonaMaterialPrecios { get; set; }

    public virtual DbSet<PersonaMaterialTratamiento> PersonaMaterialTratamientos { get; set; }

    public virtual DbSet<PersonaServicio> PersonaServicios { get; set; }

    public virtual DbSet<PersonaTipoResiduo> PersonaTipoResiduos { get; set; }

    public virtual DbSet<PersonaTratamiento> PersonaTratamientos { get; set; }

    public virtual DbSet<PersonaVehiculo> PersonaVehiculos { get; set; }

    public virtual DbSet<Pictograma> Pictogramas { get; set; }

    public virtual DbSet<PlaneacionResponsable> PlaneacionResponsables { get; set; }

    public virtual DbSet<ReferenciaResiduo> ReferenciaResiduos { get; set; }

    public virtual DbSet<Referencium> Referencia { get; set; }

    public virtual DbSet<Relacion> Relacions { get; set; }

    public virtual DbSet<Reporte> Reportes { get; set; }

    public virtual DbSet<Residuo> Residuos { get; set; }

    public virtual DbSet<ResiduoTransformacion> ResiduoTransformacions { get; set; }

    public virtual DbSet<RutaDeposito> RutaDepositos { get; set; }

    public virtual DbSet<RutaResponsablePeriodo> RutaResponsablePeriodos { get; set; }

    public virtual DbSet<Rutum> Ruta { get; set; }

    public virtual DbSet<Saldo> Saldos { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<ServicioItem> ServicioItems { get; set; }

    public virtual DbSet<Solicitud> Solicituds { get; set; }

    public virtual DbSet<SolicitudDetalle> SolicitudDetalles { get; set; }

    public virtual DbSet<SolicitudDetalleInsumo> SolicitudDetalleInsumos { get; set; }

    public virtual DbSet<SolicitudDetalleParticipacion> SolicitudDetalleParticipacions { get; set; }

    public virtual DbSet<SolicitudDetalleServicio> SolicitudDetalleServicios { get; set; }

    public virtual DbSet<SolicitudInsumo> SolicitudInsumos { get; set; }

    public virtual DbSet<Tarifa> Tarifas { get; set; }

    public virtual DbSet<TarifaCuentum> TarifaCuenta { get; set; }

    public virtual DbSet<TarifaFacturacion> TarifaFacturacions { get; set; }

    public virtual DbSet<TarifaOpcion> TarifaOpcions { get; set; }

    public virtual DbSet<TipoResiduo> TipoResiduos { get; set; }

    public virtual DbSet<TipoResiduoCaracteristica> TipoResiduoCaracteristicas { get; set; }

    public virtual DbSet<TipoResiduoClasificacion> TipoResiduoClasificacions { get; set; }

    public virtual DbSet<TipoResiduoFrase> TipoResiduoFrases { get; set; }

    public virtual DbSet<TipoResiduoHomologacion> TipoResiduoHomologacions { get; set; }

    public virtual DbSet<TipoResiduoItem> TipoResiduoItems { get; set; }

    public virtual DbSet<TipoResiduoPictograma> TipoResiduoPictogramas { get; set; }

    public virtual DbSet<TipoResiduoTratamiento> TipoResiduoTratamientos { get; set; }

    public virtual DbSet<Tratamiento> Tratamientos { get; set; }

    public virtual DbSet<TratamientoItem> TratamientoItems { get; set; }

    public virtual DbSet<Unidad> Unidads { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioDeposito> UsuarioDepositos { get; set; }

    public virtual DbSet<UsuarioOpcion> UsuarioOpcions { get; set; }

    public virtual DbSet<UsuarioPersona> UsuarioPersonas { get; set; }

    public virtual DbSet<UsuarioVehiculo> UsuarioVehiculos { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    public virtual DbSet<VwCertificadoSolicitud> VwCertificadoSolicituds { get; set; }

    public virtual DbSet<VwDeposito> VwDepositos { get; set; }

    public virtual DbSet<VwDepositoOk> VwDepositoOks { get; set; }

    public virtual DbSet<VwEmbalaje> VwEmbalajes { get; set; }

    public virtual DbSet<VwEstado> VwEstados { get; set; }

    public virtual DbSet<VwEtapa> VwEtapas { get; set; }

    public virtual DbSet<VwFase> VwFases { get; set; }

    public virtual DbSet<VwInformacionMaterial> VwInformacionMaterials { get; set; }

    public virtual DbSet<VwMaterial> VwMaterials { get; set; }

    public virtual DbSet<VwMaterialCodigoDescripcion> VwMaterialCodigoDescripcions { get; set; }

    public virtual DbSet<VwMaterialPersona> VwMaterialPersonas { get; set; }

    public virtual DbSet<VwPaisDepartamentoMunicipio> VwPaisDepartamentoMunicipios { get; set; }

    public virtual DbSet<VwSede> VwSedes { get; set; }

    public virtual DbSet<VwSolicitudParticion> VwSolicitudParticions { get; set; }

    public virtual DbSet<VwTipoResiduo> VwTipoResiduos { get; set; }

    public virtual DbSet<VwVehiculo> VwVehiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ajuste>(entity =>
        {
            entity.Property(e => e.IdAjuste).ValueGeneratedNever();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue("A");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.AjusteIdDepositoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ajuste_Deposito");

            entity.HasOne(d => d.IdDepositoDestinoNavigation).WithMany(p => p.AjusteIdDepositoDestinoNavigations).HasConstraintName("FK_Ajuste_DepositoDestino");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Ajustes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ajuste_Persona");

            entity.HasOne(d => d.IdResiduoNavigation).WithMany(p => p.Ajustes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ajuste_Residuo");
        });

        modelBuilder.Entity<Caracteristica>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Caracteristicas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Caracteristica_Categoria");
        });

        modelBuilder.Entity<Categorium>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
        });

        modelBuilder.Entity<CbStep>(entity =>
        {
            entity.HasOne(d => d.UserNavigation).WithMany(p => p.CbSteps).HasConstraintName("FK_CB_Steps_CB_Sessions");
        });

        modelBuilder.Entity<Certificado>(entity =>
        {
            entity.Property(e => e.IdCertificado)
                .ValueGeneratedNever()
                .HasComment("'T'ransporte,'D'isposición,'A'lmacenamiento,'R'ecepción,'M' Tratamiento, 'N' Transferencia");
            entity.Property(e => e.Acumulado).HasDefaultValue(false);
            entity.Property(e => e.Agrupado).HasDefaultValue(false);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue("A");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoOrigenNavigation).WithMany(p => p.Certificados).HasConstraintName("FK_Certificado_Deposito");

            entity.HasOne(d => d.IdGeneradorNavigation).WithMany(p => p.CertificadoIdGeneradorNavigations).HasConstraintName("FK_Certificado_Generador");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Certificados).HasConstraintName("FK_Certificado_Orden");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.CertificadoIdPersonaNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Certificado_Persona");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.CertificadoIdResponsableNavigations).HasConstraintName("FK_Certificado_Responsable");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.CertificadoIdSolicitanteNavigations).HasConstraintName("FK_Certificado_Solicitante");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.Certificados).HasConstraintName("FK_Certificado_Solicitud");
        });

        modelBuilder.Entity<CertificadoLicencium>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCertificadoNavigation).WithMany(p => p.CertificadoLicencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Certificado_Licencia_Certificado");

            entity.HasOne(d => d.IdLicenciaNavigation).WithMany(p => p.CertificadoLicencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Certificado_Licencia_Licencia");
        });

        modelBuilder.Entity<CertificadoPersona>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCertificadoNavigation).WithMany(p => p.CertificadoPersonas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Certificado_Persona_Certificado");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.CertificadoPersonas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Certificado_Persona_Persona");
        });

        modelBuilder.Entity<CertificadoResiduo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCertificadoNavigation).WithMany(p => p.CertificadoResiduos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CertificadoResiduo_Certificado");

            entity.HasOne(d => d.IdResiduoNavigation).WithMany(p => p.CertificadoResiduos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CertificadoResiduo_Residuo");
        });

        modelBuilder.Entity<Clasificacion>(entity =>
        {
            entity.HasKey(e => e.IdClasificacion).HasName("PK_ClaseResiduo");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Clasificacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clasificacion_Categoria");
        });

        modelBuilder.Entity<ClasificacionItem>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdClasificacionNavigation).WithMany(p => p.ClasificacionItemIdClasificacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clasificacion_Item_Clasificacion");

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.ClasificacionItemIdItemNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clasificacion_Item_Item");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.ClasificacionItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Clasificacion_Item_Relacion");
        });

        modelBuilder.Entity<Codificacion>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.IdCategoria }).HasName("PK_Homologacion");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
        });

        modelBuilder.Entity<CuentaCertificado>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCertificadoNavigation).WithMany(p => p.CuentaCertificados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Certificado_Certificado");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.CuentaCertificados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Certificado_Cuenta");
        });

        modelBuilder.Entity<CuentaInterfaz>(entity =>
        {
            entity.Property(e => e.Clave).IsFixedLength();

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.CuentaInterfazs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Interfaz_Cuenta");
        });

        modelBuilder.Entity<CuentaOpcion>(entity =>
        {
            entity.Property(e => e.EnviarCorreo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdCuentaCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.CuentaOpcions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Opcion_Cuenta");

            entity.HasOne(d => d.IdOpcionNavigation).WithMany(p => p.CuentaOpcions).HasConstraintName("FK_Cuenta_Opcion_Opcion");
        });

        modelBuilder.Entity<CuentaParametro>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.CuentaParametros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Parametro_Cuenta");

            entity.HasOne(d => d.IdParametroNavigation).WithMany(p => p.CuentaParametros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Parametro_Parametro");

            entity.HasOne(d => d.IdUsuarioCreacionNavigation).WithMany(p => p.CuentaParametroIdUsuarioCreacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Parametro_UsuarioCreacion");

            entity.HasOne(d => d.IdUsuarioUltimaModificacionNavigation).WithMany(p => p.CuentaParametroIdUsuarioUltimaModificacionNavigations).HasConstraintName("FK_Cuenta_Parametro_UsuarioUltimaModificacion");
        });

        modelBuilder.Entity<CuentaReferencium>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Autonumerar).HasDefaultValue(false);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.NumeroReferenciaInicio).HasDefaultValue(1L);
            entity.Property(e => e.ReferenciaPorItem).HasDefaultValue(false);
        });

        modelBuilder.Entity<Cuentum>(entity =>
        {
            entity.Property(e => e.IdCuenta).ValueGeneratedNever();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue("A");
            entity.Property(e => e.IdPersona).HasDefaultValueSql("((1))");
            entity.Property(e => e.IdRol).HasDefaultValue("S");
            entity.Property(e => e.IdUsuario)
                .HasDefaultValue(1L)
                .HasComment("Usuario propietario de la cuenta");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Cuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Persona");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Cuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Usuario");
        });

        modelBuilder.Entity<Deposito>(entity =>
        {
            entity.Property(e => e.IdDeposito).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdLocalizacionNavigation).WithMany(p => p.Depositos).HasConstraintName("FK_Deposito_Localizacion");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Depositos).HasConstraintName("FK_Deposito_Persona");

            entity.HasOne(d => d.IdSuperiorNavigation).WithMany(p => p.InverseIdSuperiorNavigation).HasConstraintName("FK_Deposito_Superior");
        });

        modelBuilder.Entity<DepositoContacto>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdContactoNavigation).WithMany(p => p.DepositoContactos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_Contacto_Persona");

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.DepositoContactos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_Contacto_Deposito");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.DepositoContactos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_Contacto_Relacion");
        });

        modelBuilder.Entity<DepositoLocalizacion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.DepositoLocalizacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_Localizacion_Deposito");

            entity.HasOne(d => d.IdLocalizacionNavigation).WithMany(p => p.DepositoLocalizacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_Localizacion_Localizacion");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.DepositoLocalizacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_Localizacion_Relacion");
        });

        modelBuilder.Entity<DepositoTipoResiduo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.DepositoTipoResiduos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_TipoResiduo_Deposito");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.DepositoTipoResiduos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_TipoResiduo_Relacion");

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.DepositoTipoResiduos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_TipoResiduo_Residuo");
        });

        modelBuilder.Entity<DepositoVehiculo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.DepositoVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_Vehiculo_Deposito");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.DepositoVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_Vehiculo_Relacion");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.DepositoVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deposito_Vehiculo_Vehiculo");
        });

        modelBuilder.Entity<Embalaje>(entity =>
        {
            entity.Property(e => e.IdEmbalaje).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.Publico).HasDefaultValue(true);

            entity.HasOne(d => d.IdEmbalajeSuperiorNavigation).WithMany(p => p.InverseIdEmbalajeSuperiorNavigation).HasConstraintName("FK_Embalaje_EmbalajeSuperior");

            entity.HasOne(d => d.IdUsuarioCreacionNavigation).WithMany(p => p.EmbalajeIdUsuarioCreacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Embalaje_UsuarioCreacion");

            entity.HasOne(d => d.IdUsuarioUltimaModificacionNavigation).WithMany(p => p.EmbalajeIdUsuarioUltimaModificacionNavigations).HasConstraintName("FK_Embalaje_UsuarioUltimaModificacion");
        });

        modelBuilder.Entity<Frase>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Frases)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Frase_Categoria");
        });

        modelBuilder.Entity<Gestion>(entity =>
        {
            entity.Property(e => e.IdMovimiento).ValueGeneratedNever();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdCertificadoNavigation).WithMany(p => p.Gestions).HasConstraintName("FK_Gestion_Certificado");

            entity.HasOne(d => d.IdDepositoDestinoNavigation).WithMany(p => p.GestionIdDepositoDestinoNavigations).HasConstraintName("FK_Gestion_DepositoDestino");

            entity.HasOne(d => d.IdDepositoOrigenNavigation).WithMany(p => p.GestionIdDepositoOrigenNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gestion_DepositoOrigen");

            entity.HasOne(d => d.IdMovimientoOrigenNavigation).WithMany(p => p.InverseIdMovimientoOrigenNavigation).HasConstraintName("FK_Gestion_MovimientoOrigen");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Gestions).HasConstraintName("FK_Gestion_Orden");

            entity.HasOne(d => d.IdPlantaNavigation).WithMany(p => p.GestionIdPlantaNavigations).HasConstraintName("FK_Gestion_Planta");

            entity.HasOne(d => d.IdResiduoNavigation).WithMany(p => p.Gestions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gestion_Residuo");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.Gestions).HasConstraintName("FK_Gestion_Responsable");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.Gestions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gestion_Servicio");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.Gestions).HasConstraintName("FK_Gestion_Tratamiento");

            entity.HasOne(d => d.IdUsuarioCreacionNavigation).WithMany(p => p.GestionIdUsuarioCreacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gestion_UsuarioCreacion");

            entity.HasOne(d => d.IdUsuarioUltimaModificacionNavigation).WithMany(p => p.GestionIdUsuarioUltimaModificacionNavigations).HasConstraintName("FK_Gestion_UsuarioUltimaModificacion");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Gestions).HasConstraintName("FK_Gestion_Vehiculo");
        });

        modelBuilder.Entity<Homologacion>(entity =>
        {
            entity.HasKey(e => new { e.IdCuenta, e.IdPersona }).HasName("PK_Homologacion_1");
        });

        modelBuilder.Entity<Insumo>(entity =>
        {
            entity.Property(e => e.IdInsumo).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.Publico).HasDefaultValue(true);

            entity.HasOne(d => d.IdInsumoSuperiorNavigation).WithMany(p => p.InverseIdInsumoSuperiorNavigation).HasConstraintName("FK_Insumo_InsumoSuperior");
        });

        modelBuilder.Entity<Licencium>(entity =>
        {
            entity.Property(e => e.IdLicencia).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
        });

        modelBuilder.Entity<ListaItem>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdListaNavigation).WithMany(p => p.ListaItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ListaItem_Lista");
        });

        modelBuilder.Entity<Listum>(entity =>
        {
            entity.Property(e => e.IdLista).ValueGeneratedNever();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
        });

        modelBuilder.Entity<Localizacion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.UbicacionDescripcion).HasComputedColumnSql("([Ubicacion].[STAsText]())", false);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Localizacions).HasConstraintName("FK_Localizacion_Categoria");
        });

        modelBuilder.Entity<LocalizacionItem>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.LocalizacionItemIdItemNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Localizacion_Item_Item");

            entity.HasOne(d => d.IdLocalizacionNavigation).WithMany(p => p.LocalizacionItemIdLocalizacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Localizacion_Item_Localizacion");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.LocalizacionItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Localizacion_Item_Relacion");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_dbo.Log");

            entity.Property(e => e.Level).HasDefaultValue("Info");
            entity.Property(e => e.Logged).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.Property(e => e.IdMaterial).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.Medicion).HasDefaultValue("P");
            entity.Property(e => e.Publico).HasDefaultValue(true);

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.Materials).HasConstraintName("FK_Material_TipoResiduo");
        });

        modelBuilder.Entity<MaterialItem>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.MaterialItemIdItemNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Material_Item_Material");

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.MaterialItemIdMaterialNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Material_Item_Item");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.MaterialItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Material_Item_Relacion");
        });

        modelBuilder.Entity<Mensaje>(entity =>
        {
            entity.Property(e => e.IdMensaje).ValueGeneratedNever();
            entity.Property(e => e.Fecha).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue("A");
            entity.Property(e => e.IdTipo).HasDefaultValue("M");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.Leido).HasDefaultValue(false);
            entity.Property(e => e.Receptores).HasComment("JSON Con arreglo de usuarios, marcas de lectura y otras marcas que cada receptor coloca al mensaje");

            entity.HasOne(d => d.IdCertificadoNavigation).WithMany(p => p.Mensajes).HasConstraintName("FK_Mensaje_Certificado");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.Mensajes).HasConstraintName("FK_Mensaje_Orden");

            entity.HasOne(d => d.IdPersonaEmisorNavigation).WithMany(p => p.MensajeIdPersonaEmisorNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mensaje_Emisor");

            entity.HasOne(d => d.IdPersonaReceptorNavigation).WithMany(p => p.MensajeIdPersonaReceptorNavigations).HasConstraintName("FK_Mensaje_Receptor");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.Mensajes).HasConstraintName("FK_Mensaje_Solicitud");
        });

        modelBuilder.Entity<Monedum>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.Simbolo).HasDefaultValue("U$");
        });

        modelBuilder.Entity<Opcion>(entity =>
        {
            entity.Property(e => e.Configurable).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdRol).HasDefaultValue("S");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdOpcionSuperiorNavigation).WithMany(p => p.InverseIdOpcionSuperiorNavigation).HasConstraintName("FK_Opcion_Opcion");
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.Property(e => e.IdOrden).ValueGeneratedNever();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue("A");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.OrdenIdDepositoNavigations).HasConstraintName("FK_Orden_Deposito");

            entity.HasOne(d => d.IdDepositoDestinoNavigation).WithMany(p => p.OrdenIdDepositoDestinoNavigations).HasConstraintName("FK_Orden_DepositoDestino");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.OrdenIdPersonaNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Persona");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.OrdenIdResponsableNavigations).HasConstraintName("FK_Orden_Responsable");

            entity.HasOne(d => d.IdResponsable2Navigation).WithMany(p => p.OrdenIdResponsable2Navigations).HasConstraintName("FK_Orden_Responsable2");

            entity.HasOne(d => d.IdResponsable3Navigation).WithMany(p => p.OrdenIdResponsable3Navigations).HasConstraintName("FK_Orden_Responsable3");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.Ordens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Servicio");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.Ordens).HasConstraintName("FK_Orden_Tratamiento");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Ordens).HasConstraintName("FK_Orden_Vehiculo");
        });

        modelBuilder.Entity<OrdenInsumo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.OrdenInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Insumo_Insumo");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.OrdenInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Insumo_Orden");

            entity.HasOne(d => d.IdUnidadNavigation).WithMany(p => p.OrdenInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Insumo_Unidad");

            entity.HasOne(d => d.IdUsuarioCreacionNavigation).WithMany(p => p.OrdenInsumoIdUsuarioCreacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Insumo_UsuarioCreacion");

            entity.HasOne(d => d.IdUsuarioUltimaModificacionNavigation).WithMany(p => p.OrdenInsumoIdUsuarioUltimaModificacionNavigations).HasConstraintName("FK_Orden_Insumo_UsuarioUltimaModificacion");
        });

        modelBuilder.Entity<OrdenPlaneacion>(entity =>
        {
            entity.Property(e => e.IdEstado).HasDefaultValue("A");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.OrdenPlaneacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenPlaneacion_Orden");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.OrdenPlaneacions).HasConstraintName("FK_OrdenPlaneacion_Responsable");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.OrdenPlaneacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenPlaneacion_Solicitud");
        });

        modelBuilder.Entity<OrdenResiduo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCertificadoNavigation).WithMany(p => p.OrdenResiduos).HasConstraintName("FK_OrdenResiduo_Certificado");

            entity.HasOne(d => d.IdDepositoDestinoNavigation).WithMany(p => p.OrdenResiduoIdDepositoDestinoNavigations).HasConstraintName("FK_OrdenResiduo_DepositoDestino");

            entity.HasOne(d => d.IdDepositoOrigenNavigation).WithMany(p => p.OrdenResiduoIdDepositoOrigenNavigations).HasConstraintName("FK_OrdenResiduo_Deposito");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.OrdenResiduos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduo_Orden");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.OrdenResiduoIdProveedorNavigations).HasConstraintName("FK_OrdenResiduo_Proveedor");

            entity.HasOne(d => d.IdResiduoNavigation).WithMany(p => p.OrdenResiduoIdResiduoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduo_Residuo");

            entity.HasOne(d => d.IdResiduoDestinoNavigation).WithMany(p => p.OrdenResiduoIdResiduoDestinoNavigations).HasConstraintName("FK_OrdenResiduo_ResiduoDestino");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.OrdenResiduoIdSolicitanteNavigations).HasConstraintName("FK_OrdenResiduo_Solicitante");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.OrdenResiduos).HasConstraintName("FK_OrdenResiduo_Tratamiento");
        });

        modelBuilder.Entity<OrdenResiduoInsumo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.OrdenResiduoInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduo_Insumo_Insumo");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.OrdenResiduoInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduo_Insumo_Orden");

            entity.HasOne(d => d.IdResiduoNavigation).WithMany(p => p.OrdenResiduoInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduo_Insumo_Residuo");

            entity.HasOne(d => d.IdUnidadNavigation).WithMany(p => p.OrdenResiduoInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduo_Insumo_Unidad");

            entity.HasOne(d => d.IdUsuarioCreacionNavigation).WithMany(p => p.OrdenResiduoInsumoIdUsuarioCreacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduo_Insumo_UsuarioCreacion");

            entity.HasOne(d => d.IdUsuarioUltimaModificacionNavigation).WithMany(p => p.OrdenResiduoInsumoIdUsuarioUltimaModificacionNavigations).HasConstraintName("FK_OrdenResiduo_Insumo_UsuarioUltimaModificacion");

            entity.HasOne(d => d.OrdenResiduo).WithMany(p => p.OrdenResiduoInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduo_Insumo_OrdenResiduo");
        });

        modelBuilder.Entity<OrdenResiduoTransformacion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoDestinoNavigation).WithMany(p => p.OrdenResiduoTransformacions).HasConstraintName("FK_OrdenResiduoTransformacion_Deposito");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.OrdenResiduoTransformacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduoTransformacion_Orden");

            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.OrdenResiduoTransformacions).HasConstraintName("FK_OrdenResiduoTransformacion_Propietario");

            entity.HasOne(d => d.IdResiduoNavigation).WithMany(p => p.OrdenResiduoTransformacionIdResiduoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduoTransformacion_Residuo");

            entity.HasOne(d => d.IdResiduoTransformacionNavigation).WithMany(p => p.OrdenResiduoTransformacionIdResiduoTransformacionNavigations).HasConstraintName("FK_OrdenResiduoTransformacion_ResiduoTransformacion");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.OrdenResiduoTransformacions).HasConstraintName("FK_OrdenResiduoTransformacion_Tratamiento");

            entity.HasOne(d => d.OrdenResiduo).WithMany(p => p.OrdenResiduoTransformacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenResiduoTransformacion_OrdenResiduo");
        });

        modelBuilder.Entity<Parametro>(entity =>
        {
            entity.Property(e => e.Administrable).HasDefaultValue(true);
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Personas).HasConstraintName("FK_Persona_Categoria");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.Personas).HasConstraintName("FK_Persona_Cuenta");

            entity.HasOne(d => d.IdLocalizacionNavigation).WithMany(p => p.Personas).HasConstraintName("FK_Persona_Localizacion");
        });

        modelBuilder.Entity<PersonaContacto>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.EnviarCorreo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.RequiereConciliar).HasDefaultValue(true);

            entity.HasOne(d => d.IdContactoNavigation).WithMany(p => p.PersonaContactoIdContactoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Contacto_Contacto");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.PersonaContactos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Contacto_Cuenta");

            entity.HasOne(d => d.IdLocalizacionNavigation).WithMany(p => p.PersonaContactos).HasConstraintName("FK_Persona_Contacto_Localizacion");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaContactoIdPersonaNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Contacto_Persona");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.PersonaContactos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Contacto_Relacion");
        });

        modelBuilder.Entity<PersonaEmbalaje>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.PersonaEmbalajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Embalaje_Cuenta");

            entity.HasOne(d => d.IdEmbalajeNavigation).WithMany(p => p.PersonaEmbalajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Embalaje_Embalaje");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaEmbalajes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Embalaje_Persona");
        });

        modelBuilder.Entity<PersonaInsumo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.PersonaInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Insumo_Cuenta");

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.PersonaInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Insumo_Insumo");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Insumo_Persona");
        });

        modelBuilder.Entity<PersonaLicencium>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.PersonaLicencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Licencia_Cuenta");

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.PersonaLicencia).HasConstraintName("FK_Persona_Licencia_Deposito");

            entity.HasOne(d => d.IdLicenciaNavigation).WithMany(p => p.PersonaLicencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Licencia_Licencia");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaLicencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Licencia_Persona");
        });

        modelBuilder.Entity<PersonaLocalizacion>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdLocalizacionNavigation).WithMany(p => p.PersonaLocalizacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Localizacion_Localizacion");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaLocalizacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Localizacion_Persona");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.PersonaLocalizacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Localizacion_Relacion");
        });

        modelBuilder.Entity<PersonaLocalizacionArea>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaLocalizacionAreas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Localizacion_Area_Persona");
        });

        modelBuilder.Entity<PersonaLocalizacionContacto>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdContactoNavigation).WithMany(p => p.PersonaLocalizacionContactos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Localizacion_Contacto_Persona");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.PersonaLocalizacionContactos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Localizacion_Contacto_Relacion");
        });

        modelBuilder.Entity<PersonaLocalizacionDeposito>(entity =>
        {
            entity.Property(e => e.IdRelacion).HasDefaultValue("DP");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.PersonaLocalizacionDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Localizacion_Deposito_Deposito");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaLocalizacionDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Localizacion_Deposito_Persona");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.PersonaLocalizacionDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Localizacion_Deposito_Relacion");
        });

        modelBuilder.Entity<PersonaMaterial>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdEmbalajeNavigation).WithMany(p => p.PersonaMaterials).HasConstraintName("FK_Persona_Material_Embalaje");

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.PersonaMaterials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Material");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaMaterials)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Persona");
        });

        modelBuilder.Entity<PersonaMaterialDeposito>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.PersonaMaterialDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Deposito_Deposito");

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.PersonaMaterialDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Deposito_Material");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaMaterialDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Deposito_Persona");
        });

        modelBuilder.Entity<PersonaMaterialDepositoPrecio>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.PersonaMaterialDepositoPrecios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Deposito_Precio_Deposito");

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.PersonaMaterialDepositoPrecios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Deposito_Precio_Material");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaMaterialDepositoPrecios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Deposito_Precio_Persona");
        });

        modelBuilder.Entity<PersonaMaterialItem>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.PersonaMaterialItemIdItemNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Item_Item");

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.PersonaMaterialItemIdMaterialNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Item_Material");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaMaterialItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Item_Persona");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.PersonaMaterialItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Item_Relacion");
        });

        modelBuilder.Entity<PersonaMaterialPrecio>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.PersonaMaterialPrecios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Precio_Material");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaMaterialPrecios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Precio_Persona");
        });

        modelBuilder.Entity<PersonaMaterialTratamiento>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.PersonaMaterialTratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Tratamiento_Material");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaMaterialTratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Tratamiento_Persona");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.PersonaMaterialTratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Material_Tratamiento_Tratamiento");
        });

        modelBuilder.Entity<PersonaServicio>(entity =>
        {
            entity.Property(e => e.FechaInicio).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Servicio_Persona");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.PersonaServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Servicio_Servicio");
        });

        modelBuilder.Entity<PersonaTipoResiduo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaTipoResiduos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_TipoResiduo_Persona");

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.PersonaTipoResiduos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_TipoResiduo_TipoResiduo");
        });

        modelBuilder.Entity<PersonaTratamiento>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Manejado).HasDefaultValue(true);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaTratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Tratamiento_Persona");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.PersonaTratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Tratamiento_Tratamiento");
        });

        modelBuilder.Entity<PersonaVehiculo>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Vehiculo_Persona");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.PersonaVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Vehiculo_Relacion");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.PersonaVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Persona_Vehiculo_Vehiculo");
        });

        modelBuilder.Entity<Pictograma>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Pictogramas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pictograma_Categoria");
        });

        modelBuilder.Entity<PlaneacionResponsable>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.PlaneacionResponsables)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlaneacionResponsable_Deposito");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.PlaneacionResponsables)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlaneacionResponsable_Persona");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.PlaneacionResponsables).HasConstraintName("FK_PlaneacionResponsable_Vehiculo");
        });

        modelBuilder.Entity<ReferenciaResiduo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.Referencium).WithMany(p => p.ReferenciaResiduos).HasConstraintName("FK_ReferenciaResiduo_Referencia");
        });

        modelBuilder.Entity<Referencium>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue("A");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.ReferenciumIdPersonaNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Referencia_Persona");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.ReferenciumIdResponsableNavigations).HasConstraintName("FK_Referencia_Responsable");

            entity.HasOne(d => d.IdResponsable2Navigation).WithMany(p => p.ReferenciumIdResponsable2Navigations).HasConstraintName("FK_Referencia_Responsable2");

            entity.HasOne(d => d.IdResponsable3Navigation).WithMany(p => p.ReferenciumIdResponsable3Navigations).HasConstraintName("FK_Referencia_Responsable3");
        });

        modelBuilder.Entity<Relacion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
        });

        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.Property(e => e.IdReporte).ValueGeneratedNever();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Reportes).HasConstraintName("FK_Reporte_Persona");

            entity.HasOne(d => d.IdUsuarioCreacionNavigation).WithMany(p => p.ReporteIdUsuarioCreacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reporte_UsuarioCreacion");

            entity.HasOne(d => d.IdUsuarioUltimaModificacionNavigation).WithMany(p => p.ReporteIdUsuarioUltimaModificacionNavigations).HasConstraintName("FK_Reporte_UsuarioUltimaModificacion");
        });

        modelBuilder.Entity<Residuo>(entity =>
        {
            entity.Property(e => e.IdResiduo).ValueGeneratedNever();
            entity.Property(e => e.IdEstado).HasDefaultValue("A");

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.Residuos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Residuo_Material");

            entity.HasOne(d => d.IdPropietarioNavigation).WithMany(p => p.Residuos).HasConstraintName("FK_Residuo_Persona");
        });

        modelBuilder.Entity<ResiduoTransformacion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdResiduoNavigation).WithMany(p => p.ResiduoTransformacionIdResiduoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResiduoTransformacion_Residuo");

            entity.HasOne(d => d.IdResiduoTransformacionNavigation).WithMany(p => p.ResiduoTransformacionIdResiduoTransformacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResiduoTransformacion_ResiduoTransformacion");
        });

        modelBuilder.Entity<RutaDeposito>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.RutaDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ruta_Deposito_Deposito");

            entity.HasOne(d => d.IdRutaNavigation).WithMany(p => p.RutaDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ruta_Deposito_Ruta");
        });

        modelBuilder.Entity<RutaResponsablePeriodo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.RutaResponsablePeriodos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ruta_ResponsablePeriodo_Persona");

            entity.HasOne(d => d.IdRutaNavigation).WithMany(p => p.RutaResponsablePeriodos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ruta_ResponsablePeriodo_Ruta");
        });

        modelBuilder.Entity<Rutum>(entity =>
        {
            entity.Property(e => e.IdRuta).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.DiaCompleto).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.Ruta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ruta_Cuenta");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.Ruta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ruta_Persona");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Ruta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ruta_Vehiculo");
        });

        modelBuilder.Entity<Saldo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue("A");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdResiduoNavigation).WithMany(p => p.Saldos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Saldo_Residuo");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.Saldos).HasConstraintName("FK_Saldo_Tratamiento");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.Property(e => e.IdServicio).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Servicios).HasConstraintName("FK_Servicio_Categoria");
        });

        modelBuilder.Entity<ServicioItem>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.ServicioItemIdItemNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Servicio_Item_Item");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.ServicioItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Servicio_Item_Relacion");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.ServicioItemIdServicioNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Servicio_Item_Servicio");
        });

        modelBuilder.Entity<Solicitud>(entity =>
        {
            entity.Property(e => e.IdSolicitud).ValueGeneratedNever();
            entity.Property(e => e.Facturada).HasDefaultValue(false);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue("A");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.Pagada).HasDefaultValue(false);

            entity.HasOne(d => d.IdConductorNavigation).WithMany(p => p.SolicitudIdConductorNavigations).HasConstraintName("FK_Solicitud_Conductor");

            entity.HasOne(d => d.IdDepositoDestinoNavigation).WithMany(p => p.SolicitudIdDepositoDestinoNavigations).HasConstraintName("FK_Solicitud_Deposito");

            entity.HasOne(d => d.IdDepositoMedioNavigation).WithMany(p => p.SolicitudIdDepositoMedioNavigations).HasConstraintName("FK_Solicitud_DepositoMedio");

            entity.HasOne(d => d.IdDepositoOrigenNavigation).WithMany(p => p.SolicitudIdDepositoOrigenNavigations).HasConstraintName("FK_Solicitud_DepositoOrigen");

            entity.HasOne(d => d.IdGeneradorNavigation).WithMany(p => p.SolicitudIdGeneradorNavigations).HasConstraintName("FK_Solicitud_Generador");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.SolicitudIdPersonaNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitud_Persona");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.SolicitudIdProveedorNavigations).HasConstraintName("FK_Solicitud_Proveedor");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.SolicitudIdResponsableNavigations).HasConstraintName("FK_Solicitud_Responsable");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.Solicituds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitud_Servicio");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.SolicitudIdSolicitanteNavigations).HasConstraintName("FK_Solicitud_Solicitante");

            entity.HasOne(d => d.IdTransportadorNavigation).WithMany(p => p.SolicitudIdTransportadorNavigations).HasConstraintName("FK_Solicitud_Transportador");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Solicituds).HasConstraintName("FK_Solicitud_Vehiculo");
        });

        modelBuilder.Entity<SolicitudDetalle>(entity =>
        {
            entity.Property(e => e.Aceptado).HasDefaultValue(true);
            entity.Property(e => e.Conciliado).HasDefaultValue(false);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEtapa)
                .HasDefaultValue("I")
                .HasComment("'I'nicio / 'V'alidación / 'T'ransporte / 'R'ecepción / 'P'rocesamiento / 'F'inalización");
            entity.Property(e => e.IdFase)
                .HasDefaultValue("I")
                .HasComment("'I'nicio / 'P'laneación / 'E'jecución / 'C'ertificación / 'F'inalización");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.Recibido).HasDefaultValue(true);
            entity.Property(e => e.Solicitado).HasDefaultValue(true);

            entity.HasOne(d => d.IdDepositoDestinoNavigation).WithMany(p => p.SolicitudDetalleIdDepositoDestinoNavigations).HasConstraintName("FK_SolicitudDetalle_DepositoDestino");

            entity.HasOne(d => d.IdDepositoDestinoSolicitudNavigation).WithMany(p => p.SolicitudDetalleIdDepositoDestinoSolicitudNavigations).HasConstraintName("FK_SolicitudDetalle_DepositoDestinoSolicitud");

            entity.HasOne(d => d.IdDepositoOrigenNavigation).WithMany(p => p.SolicitudDetalleIdDepositoOrigenNavigations).HasConstraintName("FK_SolicitudDetalle_DepositoOrigen");

            entity.HasOne(d => d.IdEmbalajeNavigation).WithMany(p => p.SolicitudDetalleIdEmbalajeNavigations).HasConstraintName("FK_SolicitudDetalle_Embalaje");

            entity.HasOne(d => d.IdEmbalajeSolicitudNavigation).WithMany(p => p.SolicitudDetalleIdEmbalajeSolicitudNavigations).HasConstraintName("FK_SolicitudDetalle_EmbalajeSolicitud");

            entity.HasOne(d => d.IdMaterialNavigation).WithMany(p => p.SolicitudDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Material");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.SolicitudDetalleIdProveedorNavigations).HasConstraintName("FK_SolicitudDetalle_Proveedor");

            entity.HasOne(d => d.IdProveedorSolicitudNavigation).WithMany(p => p.SolicitudDetalleIdProveedorSolicitudNavigations).HasConstraintName("FK_SolicitudDetalle_ProveedorSolicitud");

            entity.HasOne(d => d.IdResiduoNavigation).WithMany(p => p.SolicitudDetalleIdResiduoNavigations).HasConstraintName("FK_SolicitudDetalle_Residuo");

            entity.HasOne(d => d.IdResiduoOrigenNavigation).WithMany(p => p.SolicitudDetalleIdResiduoOrigenNavigations).HasConstraintName("FK_SolicitudDetalle_ResiduoOrigen");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.SolicitudDetalleIdSolicitanteNavigations).HasConstraintName("FK_SolicitudDetalle_Solicitante");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.SolicitudDetalles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Solicitud");

            entity.HasOne(d => d.IdTransportadorNavigation).WithMany(p => p.SolicitudDetalleIdTransportadorNavigations).HasConstraintName("FK_SolicitudDetalle_Transportador");

            entity.HasOne(d => d.IdTransportadorSolicitudNavigation).WithMany(p => p.SolicitudDetalleIdTransportadorSolicitudNavigations).HasConstraintName("FK_SolicitudDetalle_TransportadorSolicitud");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.SolicitudDetalles).HasConstraintName("FK_SolicitudDetalle_Tratamiento");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.SolicitudDetalleIdVehiculoNavigations).HasConstraintName("FK_SolicitudDetalle_Vehiculo");

            entity.HasOne(d => d.IdVehiculoSolicitudNavigation).WithMany(p => p.SolicitudDetalleIdVehiculoSolicitudNavigations).HasConstraintName("FK_SolicitudDetalle_VehiculoSolicitud");
        });

        modelBuilder.Entity<SolicitudDetalleInsumo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.SolicitudDetalleInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Insumo_Insumo");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.SolicitudDetalleInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Insumo_Solicitud");

            entity.HasOne(d => d.IdUnidadNavigation).WithMany(p => p.SolicitudDetalleInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Insumo_Unidad");

            entity.HasOne(d => d.IdUsuarioCreacionNavigation).WithMany(p => p.SolicitudDetalleInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Insumo_UsuarioCreacion");

            entity.HasOne(d => d.SolicitudDetalle).WithMany(p => p.SolicitudDetalleInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Insumo_SolicitudDetalle");
        });

        modelBuilder.Entity<SolicitudDetalleParticipacion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoOrigenNavigation).WithMany(p => p.SolicitudDetalleParticipacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Participacion_Deposito");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.SolicitudDetalleParticipacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Participacion_Persona");

            entity.HasOne(d => d.SolicitudDetalle).WithMany(p => p.SolicitudDetalleParticipacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Participacion_SolicitudDetalle");
        });

        modelBuilder.Entity<SolicitudDetalleServicio>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.SolicitudDetalleServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Servicio_Solicitud");

            entity.HasOne(d => d.SolicitudDetalle).WithMany(p => p.SolicitudDetalleServicios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Servicio_SolicitudDetalle");
        });

        modelBuilder.Entity<SolicitudInsumo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdInsumoNavigation).WithMany(p => p.SolicitudInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitud_Insumo_Insumo");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.SolicitudInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitud_Insumo_Solicitud");

            entity.HasOne(d => d.IdUnidadNavigation).WithMany(p => p.SolicitudInsumos).HasConstraintName("FK_Solicitud_Insumo_Unidad");

            entity.HasOne(d => d.IdUsuarioCreacionNavigation).WithMany(p => p.SolicitudInsumoIdUsuarioCreacionNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Solicitud_Insumo_UsuarioCreacion");

            entity.HasOne(d => d.IdUsuarioUltimaModificacionNavigation).WithMany(p => p.SolicitudInsumoIdUsuarioUltimaModificacionNavigations).HasConstraintName("FK_Solicitud_Insumo_UsuarioUltimaModificacion");
        });

        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstado).HasDefaultValue("A");
            entity.Property(e => e.IdMoneda).HasDefaultValue(1L);
            entity.Property(e => e.IdRol).HasDefaultValue("S");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdMonedaNavigation).WithMany(p => p.Tarifas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarifa_Moneda");
        });

        modelBuilder.Entity<TarifaCuentum>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.TarifaCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarifa_Cuenta_Cuenta");

            entity.HasOne(d => d.IdTarifaNavigation).WithMany(p => p.TarifaCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarifa_Cuenta_Tarifa");
        });

        modelBuilder.Entity<TarifaFacturacion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.TarifaFacturacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarifa_Facturacion_Cuenta");

            entity.HasOne(d => d.IdMonedaNavigation).WithMany(p => p.TarifaFacturacions).HasConstraintName("FK_Tarifa_Facturacion_Moneda");

            entity.HasOne(d => d.IdTarifaNavigation).WithMany(p => p.TarifaFacturacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarifa_Facturacion_Tarifa");
        });

        modelBuilder.Entity<TarifaOpcion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdOpcionNavigation).WithMany(p => p.TarifaOpcions).HasConstraintName("FK_Tarifa_Opcion_Opcion");

            entity.HasOne(d => d.IdTarifaNavigation).WithMany(p => p.TarifaOpcions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarifa_Opcion_Tarifa");
        });

        modelBuilder.Entity<TipoResiduo>(entity =>
        {
            entity.Property(e => e.IdTipoResiduo).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.Publico).HasDefaultValue(true);
        });

        modelBuilder.Entity<TipoResiduoCaracteristica>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdCaracteristicaNavigation).WithMany(p => p.TipoResiduoCaracteristicas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Caracteristica_Caracteristica");

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.TipoResiduoCaracteristicas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Caracteristica_TipoResiduo");
        });

        modelBuilder.Entity<TipoResiduoClasificacion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdClasificacionNavigation).WithMany(p => p.TipoResiduoClasificacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Clasificacion_Clasificacion");

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.TipoResiduoClasificacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Clasificacion_TipoResiduo");
        });

        modelBuilder.Entity<TipoResiduoFrase>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdFraseNavigation).WithMany(p => p.TipoResiduoFrases)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Frase_Frase");

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.TipoResiduoFrases)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Frase_TipoResiduo");
        });

        modelBuilder.Entity<TipoResiduoHomologacion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.TipoResiduoHomologacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Homologacion_TipoResiduo");

            entity.HasOne(d => d.Codificacion).WithMany(p => p.TipoResiduoHomologacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Homologacion_Codificacion");
        });

        modelBuilder.Entity<TipoResiduoItem>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.TipoResiduoItemIdItemNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Item_Item");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.TipoResiduoItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Item_Relacion");

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.TipoResiduoItemIdTipoResiduoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Item_TipoResiduo");
        });

        modelBuilder.Entity<TipoResiduoPictograma>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdPictogramaNavigation).WithMany(p => p.TipoResiduoPictogramas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Pictograma_Pictograma");

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.TipoResiduoPictogramas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Pictograma_TipoResiduo");
        });

        modelBuilder.Entity<TipoResiduoTratamiento>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdTipoResiduoNavigation).WithMany(p => p.TipoResiduoTratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Tratamiento_TipoResiduo");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.TipoResiduoTratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoResiduo_Tratamiento_Tratamiento");
        });

        modelBuilder.Entity<Tratamiento>(entity =>
        {
            entity.Property(e => e.IdTratamiento).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
            entity.Property(e => e.Publico).HasDefaultValue(false);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Tratamientos).HasConstraintName("FK_Tratamiento_Categoria");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.Tratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tratamiento_Servicio");
        });

        modelBuilder.Entity<TratamientoItem>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdItemNavigation).WithMany(p => p.TratamientoItemIdItemNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tratamiento_Item_Item");

            entity.HasOne(d => d.IdRelacionNavigation).WithMany(p => p.TratamientoItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tratamiento_Item_Relacion");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.TratamientoItemIdTratamientoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tratamiento_Item_Tratamiento");
        });

        modelBuilder.Entity<Unidad>(entity =>
        {
            entity.Property(e => e.IdUnidad).ValueGeneratedNever();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(e => e.IdUsuario).ValueGeneratedNever();

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.UsuarioIdCuentaNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Cuenta");

            entity.HasOne(d => d.IdCuentaOrigenNavigation).WithMany(p => p.UsuarioIdCuentaOrigenNavigations).HasConstraintName("FK_Usuario_CuentaOrigen");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Usuarios).HasConstraintName("FK_Usuario_Persona");
        });

        modelBuilder.Entity<UsuarioDeposito>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdDepositoNavigation).WithMany(p => p.UsuarioDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Deposito_Deposito");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioDepositos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Deposito_Usuario");
        });

        modelBuilder.Entity<UsuarioOpcion>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Habilitado).HasDefaultValue(true);
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdOpcionNavigation).WithMany(p => p.UsuarioOpcions).HasConstraintName("FK_Usuario_Opcion_Opcion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioOpcions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Opcion_Usuario");
        });

        modelBuilder.Entity<UsuarioPersona>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.UsuarioPersonas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Persona_Persona");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioPersonas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Persona_Usuario");
        });

        modelBuilder.Entity<UsuarioVehiculo>(entity =>
        {
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Vehiculo_Usuario");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.UsuarioVehiculos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Vehiculo_Vehiculo");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdUsuarioCreacion).HasDefaultValue(1L);

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.Vehiculos).HasConstraintName("FK_Vehiculo_Persona");
        });

        modelBuilder.Entity<VwCertificadoSolicitud>(entity =>
        {
            entity.ToView("vwCertificadoSolicitud");
        });

        modelBuilder.Entity<VwDeposito>(entity =>
        {
            entity.ToView("vwDeposito");
        });

        modelBuilder.Entity<VwDepositoOk>(entity =>
        {
            entity.ToView("vwDepositoOk");
        });

        modelBuilder.Entity<VwEmbalaje>(entity =>
        {
            entity.ToView("vwEmbalaje");
        });

        modelBuilder.Entity<VwEstado>(entity =>
        {
            entity.ToView("vwEstado");
        });

        modelBuilder.Entity<VwEtapa>(entity =>
        {
            entity.ToView("vwEtapa");
        });

        modelBuilder.Entity<VwFase>(entity =>
        {
            entity.ToView("vwFase");
        });

        modelBuilder.Entity<VwInformacionMaterial>(entity =>
        {
            entity.ToView("vwInformacionMaterial");
        });

        modelBuilder.Entity<VwMaterial>(entity =>
        {
            entity.ToView("vwMaterial");
        });

        modelBuilder.Entity<VwMaterialCodigoDescripcion>(entity =>
        {
            entity.ToView("vwMaterialCodigoDescripcion");
        });

        modelBuilder.Entity<VwMaterialPersona>(entity =>
        {
            entity.ToView("vwMaterialPersona");
        });

        modelBuilder.Entity<VwPaisDepartamentoMunicipio>(entity =>
        {
            entity.ToView("vwPaisDepartamentoMunicipio");
        });

        modelBuilder.Entity<VwSede>(entity =>
        {
            entity.ToView("vwSede");
        });

        modelBuilder.Entity<VwSolicitudParticion>(entity =>
        {
            entity.ToView("vwSolicitudParticion");
        });

        modelBuilder.Entity<VwTipoResiduo>(entity =>
        {
            entity.ToView("vwTipoResiduo");
        });

        modelBuilder.Entity<VwVehiculo>(entity =>
        {
            entity.ToView("vwVehiculo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
