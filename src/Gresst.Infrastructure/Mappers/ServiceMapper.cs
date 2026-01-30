using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Service (Domain/Inglés) ↔ Servicio (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class ServiceMapper : MapperBase<Service, Servicio>
{
    /// <summary>
    /// BD → Domain: Servicio → Service
    /// </summary>
    public override Service ToDomain(Servicio dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new Service
        {
            // IDs - Domain BaseEntity uses string
            Id = IdConversion.ToStringFromLong(dbEntity.IdServicio),
            AccountId = string.Empty, // Servicio is not account-specific in BD
            
            // Basic Info
            Name = dbEntity.Nombre ?? string.Empty,
            CategoryCode = dbEntity.IdCategoria,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: Service → Servicio
    /// </summary>
    public override Servicio ToDatabase(Service domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new Servicio
        {
            // IDs
            IdServicio = IdConversion.ToLongFromString(domainEntity.Id),
            
            // Basic Info
            Nombre = domainEntity.Name,
            IdCategoria = domainEntity.CategoryCode,
            
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

    public override void UpdateDatabase(Service domainEntity, Servicio dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.IdCategoria = domainEntity.CategoryCode;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

