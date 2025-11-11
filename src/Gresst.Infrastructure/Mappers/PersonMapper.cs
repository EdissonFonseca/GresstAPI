using Gresst.Domain.Entities;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Person (Domain/Inglés) ↔ Persona (BD/Español)
/// </summary>
public class PersonMapper : MapperBase<Person, Persona>
{
    /// <summary>
    /// BD → Domain: Persona → Person
    /// </summary>
    public override Person ToDomain(Persona dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new Person
        {
            // IDs
            Id = ConvertStringToGuid(dbEntity.IdPersona),
            AccountId = dbEntity.IdCuenta.HasValue 
                ? ConvertLongToGuid(dbEntity.IdCuenta.Value) 
                : Guid.Empty,
            
            // Basic Info
            Name = dbEntity.Nombre ?? string.Empty,
            DocumentNumber = dbEntity.Identificacion,
            Email = dbEntity.Correo,
            Phone = dbEntity.Telefono,
            Address = dbEntity.Direccion,
            
            // Capabilities - Determinar desde PersonaServicio o por defecto true para todos
            IsGenerator = true, // Todos pueden generar
            IsCollector = true, // Determinar desde servicios habilitados
            IsTransporter = true,
            IsReceiver = true,
            IsDisposer = true,
            IsTreater = true,
            IsStorageProvider = true,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: Person → Persona
    /// </summary>
    public override Persona ToDatabase(Person domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new Persona
        {
            // IDs
            IdPersona = ConvertGuidToString(domainEntity.Id),
            IdCuenta = domainEntity.AccountId != Guid.Empty 
                ? ConvertGuidToLong(domainEntity.AccountId) 
                : null,
            
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

    public override void UpdateDatabase(Person domainEntity, Persona dbEntity)
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

    // Type conversion helpers
    private Guid ConvertLongToGuid(long id)
    {
        if (id == 0) return Guid.Empty;
        return new Guid(id.ToString().PadLeft(32, '0'));
    }

    private Guid ConvertStringToGuid(string id)
    {
        if (string.IsNullOrEmpty(id)) return Guid.Empty;
        
        if (Guid.TryParse(id, out var guid))
            return guid;
        
        return new Guid(id.PadLeft(32, '0').Substring(0, 32));
    }

    private long ConvertGuidToLong(Guid guid)
    {
        if (guid == Guid.Empty) return 0;
        
        var guidString = guid.ToString().Replace("-", "");
        var numericPart = new string(guidString.Where(char.IsDigit).Take(18).ToArray());
        
        return long.TryParse(numericPart, out var result) ? result : 0;
    }

    private string ConvertGuidToString(Guid guid)
    {
        if (guid == Guid.Empty) return string.Empty;
        return guid.ToString().Replace("-", "").Substring(0, 40);
    }
}

