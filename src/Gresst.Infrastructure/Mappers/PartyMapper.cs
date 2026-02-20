using Gresst.Domain.Entities;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

public class PersonaDb
{
    public string IdPersona { get; set; } = String.Empty;
    public string? IdCategoria { get; set; }
    public long? IdCuenta { get; set; }
    public string? IdTipoIdentificacion { get; set; }
    public string? IdRol { get; set; }
    public string? IdTipoPersona { get; set; }
    public string? Identificacion { get; set; }
    public int? DigitoVerificacion { get; set; }
    public string? Nombre { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Telefono2 { get; set; }
    public string? Correo { get; set; }
    public NetTopologySuite.Geometries.Geometry? UbicacionMapa { get; set; }
    public NetTopologySuite.Geometries.Geometry? UbicacionLocal { get; set; }
    public bool Activo { get; set; }
    public string? Licencia { get; set; }
    public string? Cargo { get; set; }
    public string? Pagina { get; set; }
    public string? Firma { get; set; }
    public string? IdLocalizacion { get; set; }
    public string? DatosAdicionales { get; set; }
    public long? IdUsuarioCreacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public long? IdUsuarioUltimaModificacion { get; set; }
    public DateTime? FechaUltimaModificacion { get; set; }

    public static PersonaDb FromPersona(Persona p) => new()
    {
        IdPersona = p.IdPersona,
        IdCategoria = p.IdCategoria,
        IdCuenta = p.IdCuenta,
        IdTipoIdentificacion = p.IdTipoIdentificacion,
        IdRol = p.IdRol,
        IdTipoPersona = p.IdTipoPersona,
        Identificacion = p.Identificacion,
        DigitoVerificacion = p.DigitoVerificacion,
        Nombre = p.Nombre,
        Direccion = p.Direccion,
        Telefono = p.Telefono,
        Telefono2 = p.Telefono2,
        Correo = p.Correo,
        UbicacionMapa = p.UbicacionMapa,
        UbicacionLocal = p.UbicacionLocal,
        Activo = p.Activo,
        Licencia = p.Licencia,
        Cargo = p.Cargo,
        Pagina = p.Pagina,
        Firma = p.Firma,
        IdLocalizacion = p.IdLocalizacion,
        DatosAdicionales = p.DatosAdicionales,
        IdUsuarioCreacion = p.IdUsuarioCreacion,
        FechaCreacion = p.FechaCreacion,
        IdUsuarioUltimaModificacion = p.IdUsuarioUltimaModificacion,
        FechaUltimaModificacion = p.FechaUltimaModificacion,
    };

    public Persona ToPersona() => new()
    {
        IdPersona = IdPersona,
        IdCategoria = IdCategoria,
        IdCuenta = IdCuenta,
        IdTipoIdentificacion = IdTipoIdentificacion,
        IdRol = IdRol,
        IdTipoPersona = IdTipoPersona,
        Identificacion = Identificacion,
        DigitoVerificacion = DigitoVerificacion,
        Nombre = Nombre,
        Direccion = Direccion,
        Telefono = Telefono,
        Telefono2 = Telefono2,
        Correo = Correo,
        UbicacionMapa = UbicacionMapa,
        UbicacionLocal = UbicacionLocal,
        Activo = Activo,
        Licencia = Licencia,
        Cargo = Cargo,
        Pagina = Pagina,
        Firma = Firma,
        IdLocalizacion = IdLocalizacion,
        DatosAdicionales = DatosAdicionales,
        IdUsuarioCreacion = IdUsuarioCreacion ?? 0,
        FechaCreacion = FechaCreacion,
        IdUsuarioUltimaModificacion = IdUsuarioUltimaModificacion,
        FechaUltimaModificacion = FechaUltimaModificacion,
    };
}

/// <summary>
/// Mapper: Person (Domain/Inglés) ↔ Persona (BD/Español)
/// </summary>
public class PartyMapper : MapperBase<Party, PersonaDb>
{

    
    /// <summary>
         /// BD → Domain: PersonaDb → Person
         /// </summary>
    public override Party ToDomain(PersonaDb dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        try
        {
            return new Party
            {
                // IDs
                Id = dbEntity.IdPersona ?? string.Empty,
                AccountId = dbEntity.IdCuenta?.ToString() ?? string.Empty,

                // Basic Info
                Name = dbEntity.Nombre ?? string.Empty,
                DocumentNumber = dbEntity.Identificacion,
                Email = dbEntity.Correo,
                Phone = dbEntity.Telefono,
                Address = dbEntity.Direccion,

                Roles = new List<PartyRelationType>{
                    dbEntity.IdRol switch
                    {
                        "CL" => PartyRelationType.Customer,
                        "EM" => PartyRelationType.Employee,
                        _ => PartyRelationType.Unknown,
                    }
                },

                // Audit
                CreatedAt = dbEntity.FechaCreacion,
                UpdatedAt = dbEntity.FechaUltimaModificacion,
                CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
                UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
                IsActive = dbEntity.Activo

            };
        }
        catch (Exception ex)
        {
            var str = ex.Message;
            return new Party();
        }
    }

    /// <summary>
    /// Domain → BD: Person → Persona
    /// </summary>
    public override PersonaDb ToDatabase(Party domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaDb
        {
            // IDs
            IdPersona = domainEntity.Id,
            IdCuenta = string.IsNullOrEmpty(domainEntity.AccountId) ? null : long.Parse(domainEntity.AccountId),
            
            // Basic Info
            Nombre = domainEntity.Name,
            Identificacion = domainEntity.DocumentNumber,
            Correo = domainEntity.Email,
            Telefono = domainEntity.Phone,
            Direccion = domainEntity.Address,
            
            // Defaults for required fields
            IdRol = "01", // Default role
            IdTipoPersona = "J", // Jurídica por defecto
            
            // Status
            Activo = domainEntity.IsActive,
            
            // Audit
            FechaCreacion = domainEntity.CreatedAt,
            IdUsuarioCreacion = !string.IsNullOrEmpty(domainEntity.CreatedBy) 
                ? long.Parse(domainEntity.CreatedBy) 
                : 0,
            FechaUltimaModificacion = domainEntity.UpdatedAt,
            IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
                ? long.Parse(domainEntity.UpdatedBy) 
                : null
        };
    }

    public override void UpdateDatabase(Party domainEntity, PersonaDb dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Identificacion = domainEntity.DocumentNumber;
        dbEntity.Correo = domainEntity.Email;
        dbEntity.Telefono = domainEntity.Phone;
        dbEntity.Direccion = domainEntity.Address;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }

}

