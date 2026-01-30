using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: PersonSupply (Domain/Inglés) ↔ PersonaInsumo (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class PersonSupplyMapper : MapperBase<PersonSupply, PersonaInsumo>
{
    /// <summary>
    /// BD → Domain: PersonaInsumo → PersonSupply
    /// </summary>
    public override PersonSupply ToDomain(PersonaInsumo dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new PersonSupply
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid().ToString(),
            AccountId = dbEntity.IdCuenta.ToString(),
            
            // Relations
            PersonId = dbEntity.IdPersona ?? string.Empty,
            SupplyId = IdConversion.ToStringFromLong(dbEntity.IdInsumo),
            
            // Properties
            Price = dbEntity.Precio,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = true // PersonaInsumo doesn't have Activo field, assume true
        };
    }

    /// <summary>
    /// Domain → BD: PersonSupply → PersonaInsumo
    /// </summary>
    public override PersonaInsumo ToDatabase(PersonSupply domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaInsumo
        {
            // IDs (composite key)
            IdPersona = domainEntity.PersonId ?? string.Empty,
            IdInsumo = IdConversion.ToLongFromString(domainEntity.SupplyId),
            IdCuenta = long.TryParse(domainEntity.AccountId, out var psAc) ? psAc : 0,
            
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

    public override void UpdateDatabase(PersonSupply domainEntity, PersonaInsumo dbEntity)
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

