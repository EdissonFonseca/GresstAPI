using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using DomainPackaging = Gresst.Domain.Entities.Packaging;
using DbPackaging = Gresst.Infrastructure.Data.Entities.Embalaje;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Packaging (Domain/Inglés) ↔ Embalaje (BD/Español)
/// </summary>
public class PackagingMapper : MapperBase<DomainPackaging, DbPackaging>
{
    /// <summary>
    /// BD → Domain: Embalaje → Packaging
    /// </summary>
    public override DomainPackaging ToDomain(DbPackaging dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new DomainPackaging
        {
            // IDs - Domain BaseEntity uses string
            Id = dbEntity.IdEmbalaje != 0 
                ? IdConversion.ToStringFromLong(dbEntity.IdEmbalaje) 
                : string.Empty,
            
            // Basic Info
            Code = dbEntity.IdEmbalaje.ToString(), // Usar ID como código si no hay código específico
            Name = dbEntity.Nombre ?? string.Empty,
            Description = null, // No está en BD
            
            // Type - No está en BD, usar valor por defecto
            PackagingType = string.Empty,
            
            // Capacity - No está en BD
            Capacity = null,
            CapacityUnit = null,
            
            // Properties
            IsReusable = false, // No está en BD, valor por defecto
            Material = null, // No está en BD
            
            // UN Packaging codes - No está en BD
            UNPackagingCode = null,
            
            // Audit fields
            AccountId = string.Empty, // Embalaje no tiene IdCuenta directo
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: Packaging → Embalaje
    /// </summary>
    public override DbPackaging ToDatabase(DomainPackaging domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new DbPackaging
        {
            // IDs - Domain Id is string
            IdEmbalaje = IdConversion.ToLongFromString(domainEntity.Id),
            
            // Parent packaging (hierarchical structure)
            IdEmbalajeSuperior = null, // Se puede agregar si se necesita
            
            // Basic Info
            Nombre = domainEntity.Name,
            
            // Properties
            Activo = domainEntity.IsActive,
            Publico = false, // Por defecto, se puede configurar si es necesario
            
            // Audit
            IdUsuarioCreacion = !string.IsNullOrEmpty(domainEntity.CreatedBy) 
                ? long.Parse(domainEntity.CreatedBy) 
                : 0,
            FechaCreacion = domainEntity.CreatedAt,
            IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
                ? long.Parse(domainEntity.UpdatedBy) 
                : null,
            FechaUltimaModificacion = domainEntity.UpdatedAt
        };
    }

    public override void UpdateDatabase(DomainPackaging domainEntity, DbPackaging dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

