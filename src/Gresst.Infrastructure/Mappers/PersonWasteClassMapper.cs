using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: PersonWasteClass (Domain/Inglés) ↔ PersonaTipoResiduo (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class PersonWasteClassMapper : MapperBase<PersonWasteClass, PersonaTipoResiduo>
{
    /// <summary>
    /// BD → Domain: PersonaTipoResiduo → PersonWasteClass
    /// </summary>
    public override PersonWasteClass ToDomain(PersonaTipoResiduo dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        // Convert int IdTipoResiduo to string (domain ID)
        var wasteClassId = IdConversion.ToStringFromLong(dbEntity.IdTipoResiduo);

        return new PersonWasteClass
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid().ToString(),
            AccountId = dbEntity.IdCuenta.ToString(),
            
            // Relations
            PersonId = dbEntity.IdPersona ?? string.Empty,
            WasteClassId = wasteClassId,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = true // PersonaTipoResiduo doesn't have Activo field, assume true
        };
    }

    /// <summary>
    /// Domain → BD: PersonWasteClass → PersonaTipoResiduo
    /// </summary>
    public override PersonaTipoResiduo ToDatabase(PersonWasteClass domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        // Convert string WasteClassId to int (domain ID to DB int)
        var wasteClassIdLong = IdConversion.ToLongFromString(domainEntity.WasteClassId);
        var wasteClassIdInt = (int)wasteClassIdLong; // Cast long to int

        return new PersonaTipoResiduo
        {
            // IDs (composite key)
            IdPersona = domainEntity.PersonId ?? string.Empty,
            IdTipoResiduo = wasteClassIdInt,
            IdCuenta = long.TryParse(domainEntity.AccountId, out var pwcAc) ? pwcAc : 0,
            
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

    public override void UpdateDatabase(PersonWasteClass domainEntity, PersonaTipoResiduo dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // No modifiable fields (only IDs which are part of composite key)
        // Just update audit fields
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

