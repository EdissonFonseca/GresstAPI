using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Account (Domain/Inglés) ↔ Cuentum (BD/Español)
/// Maps between clean domain entity and database entity
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
            Id = GuidLongConverter.ToGuid(dbEntity.IdCuenta),
            AccountId = GuidLongConverter.ToGuid(dbEntity.IdCuenta), // Self-reference
            
            // Basic Info
            Name = dbEntity.Nombre,
            Code = dbEntity.IdCuenta.ToString(), // Use DB ID as code
            Role = MapRole(dbEntity.IdRol),
            Status = MapStatus(dbEntity.IdEstado),
            
            // Relations
            PersonId = GuidStringConverter.ToGuid(dbEntity.IdPersona),
            ParentAccountId = null, // Cuentum doesn't have parent relationship
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.IdEstado == "A"
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
            IdCuenta = GuidLongConverter.ToLong(domainEntity.Id),
            
            // Basic Info
            Nombre = domainEntity.Name,
            IdRol = MapRoleToDb(domainEntity.Role),
            IdEstado = MapStatusToDb(domainEntity.Status),
            
            // Relations
            IdPersona = GuidStringConverter.ToString(domainEntity.PersonId),
            IdUsuario = 0, // Usuario is for authentication, not directly from domain
            
            // Technical fields (in database but not in domain)
            Ajustes = null,
            PermisosPorSede = false,
            
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

    /// <summary>
    /// Update existing database entity with domain values
    /// </summary>
    public override void UpdateDatabase(Account domainEntity, Cuentum dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable business fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.IdRol = MapRoleToDb(domainEntity.Role);
        dbEntity.IdEstado = MapStatusToDb(domainEntity.Status);
        dbEntity.IdPersona = GuidStringConverter.ToString(domainEntity.PersonId);
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }

    // Role mapping
    private AccountRole MapRole(string dbRole)
    {
        return dbRole?.ToUpper() switch
        {
            "N" => AccountRole.Generator,      // "N" = Generador
            "S" => AccountRole.Operator,       // "S" = Operador logístico (Servicio)
            "B" => AccountRole.Both,           // "B" = Ambos
            _ => AccountRole.Generator
        };
    }

    private string MapRoleToDb(AccountRole role)
    {
        return role switch
        {
            AccountRole.Generator => "N",
            AccountRole.Operator => "S",
            AccountRole.Both => "B",
            _ => "N"
        };
    }

    // Status mapping
    private AccountStatus MapStatus(string? dbStatus)
    {
        return dbStatus?.ToUpper() switch
        {
            "A" => AccountStatus.Active,
            "I" => AccountStatus.Inactive,
            "S" => AccountStatus.Suspended,
            "B" => AccountStatus.Blocked,
            _ => AccountStatus.Inactive
        };
    }

    private string MapStatusToDb(AccountStatus status)
    {
        return status switch
        {
            AccountStatus.Active => "A",
            AccountStatus.Inactive => "I",
            AccountStatus.Suspended => "S",
            AccountStatus.Blocked => "B",
            _ => "I"
        };
    }
}
