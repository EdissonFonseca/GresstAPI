using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: PersonTreatment (Domain/Inglés) ↔ PersonaTratamiento (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class PersonTreatmentMapper : MapperBase<PersonTreatment, PersonaTratamiento>
{
    /// <summary>
    /// BD → Domain: PersonaTratamiento → PersonTreatment
    /// </summary>
    public override PersonTreatment ToDomain(PersonaTratamiento dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new PersonTreatment
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid().ToString(),
            AccountId = dbEntity.IdCuenta.ToString(),
            
            // Relations
            PersonId = dbEntity.IdPersona ?? string.Empty,
            TreatmentId = IdConversion.ToStringFromLong(dbEntity.IdTratamiento),
            
            // Properties
            IsManaged = dbEntity.Manejado,
            CanTransfer = dbEntity.Transferido,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: PersonTreatment → PersonaTratamiento
    /// </summary>
    public override PersonaTratamiento ToDatabase(PersonTreatment domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaTratamiento
        {
            // IDs (composite key)
            IdPersona = domainEntity.PersonId ?? string.Empty,
            IdTratamiento = IdConversion.ToLongFromString(domainEntity.TreatmentId),
            IdCuenta = long.TryParse(domainEntity.AccountId, out var ptAc) ? ptAc : 0,
            
            // Properties
            Manejado = domainEntity.IsManaged,
            Transferido = domainEntity.CanTransfer,
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

    public override void UpdateDatabase(PersonTreatment domainEntity, PersonaTratamiento dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.Manejado = domainEntity.IsManaged;
        dbEntity.Transferido = domainEntity.CanTransfer;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

