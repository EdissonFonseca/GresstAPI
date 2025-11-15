using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: PersonPackaging (Domain/Inglés) ↔ PersonaEmbalaje (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class PersonPackagingMapper : MapperBase<PersonPackaging, PersonaEmbalaje>
{
    /// <summary>
    /// BD → Domain: PersonaEmbalaje → PersonPackaging
    /// </summary>
    public override PersonPackaging ToDomain(PersonaEmbalaje dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new PersonPackaging
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid(),
            AccountId = GuidLongConverter.ToGuid(dbEntity.IdCuenta),
            
            // Relations
            PersonId = GuidStringConverter.ToGuid(dbEntity.IdPersona),
            PackagingId = GuidLongConverter.ToGuid(dbEntity.IdEmbalaje),
            
            // Properties
            Price = dbEntity.Precio,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = true // PersonaEmbalaje doesn't have Activo field, assume always active
        };
    }

    /// <summary>
    /// Domain → BD: PersonPackaging → PersonaEmbalaje
    /// </summary>
    public override PersonaEmbalaje ToDatabase(PersonPackaging domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaEmbalaje
        {
            // IDs (composite key)
            IdPersona = GuidStringConverter.ToString(domainEntity.PersonId),
            IdEmbalaje = GuidLongConverter.ToLong(domainEntity.PackagingId),
            IdCuenta = GuidLongConverter.ToLong(domainEntity.AccountId),
            
            // Properties
            Precio = domainEntity.Price,
            
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

    public override void UpdateDatabase(PersonPackaging domainEntity, PersonaEmbalaje dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.Precio = domainEntity.Price;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

