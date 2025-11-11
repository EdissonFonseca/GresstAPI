using Gresst.Domain.Entities;
using Gresst.Infrastructure.Data.Entities;
using System.Text.Json;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Account (Domain/Inglés) ↔ Cuentum (BD/Español)
/// </summary>
public class AccountMapper : MapperBase<Account, Cuentum>
{
    /// <summary>
    /// BD → Domain: Cuentum → Account
    /// </summary>
    public override Account ToDomain(Cuentum dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new Account
        {
            // IDs
            Id = ConvertLongToGuid(dbEntity.IdCuenta),
            AccountId = ConvertLongToGuid(dbEntity.IdCuenta), // La cuenta se referencia a sí misma
            
            // Basic Info
            Name = dbEntity.Nombre,
            Role = dbEntity.IdRol,
            Status = dbEntity.IdEstado,
            
            // Relations
            PersonId = ConvertStringToGuid(dbEntity.IdPersona),
            AdministratorId = ConvertLongToGuid(dbEntity.IdUsuario),
            
            // Configuration
            Settings = !string.IsNullOrEmpty(dbEntity.Ajustes) 
                ? JsonSerializer.Deserialize<Dictionary<string, string>>(dbEntity.Ajustes) ?? new Dictionary<string, string>()
                : new Dictionary<string, string>(),
            Parameters = new Dictionary<string, string>(), // Se cargaría desde CuentaParametro si es necesario
            
            // Properties
            PermissionsBySite = dbEntity.PermisosPorSede,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.IdEstado != "I" // "I" = Inactivo
        };
    }

    /// <summary>
    /// Domain → BD: Account → Cuentum
    /// </summary>
    public override Cuentum ToDatabase(Account domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new Cuentum
        {
            // IDs
            IdCuenta = ConvertGuidToLong(domainEntity.Id),
            
            // Basic Info
            Nombre = domainEntity.Name,
            IdRol = domainEntity.Role,
            IdEstado = domainEntity.Status ?? "A", // "A" = Activo
            
            // Relations
            IdPersona = ConvertGuidToString(domainEntity.PersonId),
            IdUsuario = ConvertGuidToLong(domainEntity.AdministratorId),
            
            // Configuration
            Ajustes = domainEntity.Settings.Any() 
                ? JsonSerializer.Serialize(domainEntity.Settings)
                : null,
            
            // Properties
            PermisosPorSede = domainEntity.PermissionsBySite,
            
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

    public override void UpdateDatabase(Account domainEntity, Cuentum dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.IdRol = domainEntity.Role;
        dbEntity.IdEstado = domainEntity.Status ?? "A";
        dbEntity.PermisosPorSede = domainEntity.PermissionsBySite;
        
        // Configuration
        dbEntity.Ajustes = domainEntity.Settings.Any() 
            ? JsonSerializer.Serialize(domainEntity.Settings)
            : null;
        
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
        
        var hexString = new string(id.Where(c => char.IsLetterOrDigit(c)).ToArray());
        hexString = hexString.PadLeft(32, '0').Substring(0, 32);
        
        var formatted = $"{hexString.Substring(0, 8)}-{hexString.Substring(8, 4)}-{hexString.Substring(12, 4)}-{hexString.Substring(16, 4)}-{hexString.Substring(20, 12)}";
        
        return Guid.Parse(formatted);
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

