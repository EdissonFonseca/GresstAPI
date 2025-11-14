using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: PersonMaterialTreatment (Domain/Inglés) ↔ PersonaMaterialTratamiento (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class PersonMaterialTreatmentMapper : MapperBase<PersonMaterialTreatment, PersonaMaterialTratamiento>
{
    /// <summary>
    /// BD → Domain: PersonaMaterialTratamiento → PersonMaterialTreatment
    /// </summary>
    public override PersonMaterialTreatment ToDomain(PersonaMaterialTratamiento dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new PersonMaterialTreatment
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid(),
            AccountId = GuidLongConverter.ToGuid(dbEntity.IdCuenta),
            
            // Relations
            PersonId = GuidLongConverter.StringToGuid(dbEntity.IdPersona),
            MaterialId = GuidLongConverter.ToGuid(dbEntity.IdMaterial),
            TreatmentId = GuidLongConverter.ToGuid(dbEntity.IdTratamiento),
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: PersonMaterialTreatment → PersonaMaterialTratamiento
    /// </summary>
    public override PersonaMaterialTratamiento ToDatabase(PersonMaterialTreatment domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaMaterialTratamiento
        {
            // IDs (composite key)
            IdPersona = GuidLongConverter.GuidToString(domainEntity.PersonId),
            IdMaterial = GuidLongConverter.ToLong(domainEntity.MaterialId),
            IdTratamiento = GuidLongConverter.ToLong(domainEntity.TreatmentId),
            IdCuenta = GuidLongConverter.ToLong(domainEntity.AccountId),
            
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

    public override void UpdateDatabase(PersonMaterialTreatment domainEntity, PersonaMaterialTratamiento dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

