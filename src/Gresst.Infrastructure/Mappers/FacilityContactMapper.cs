using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: FacilityContact (Domain/Inglés) ↔ DepositoContacto (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class FacilityContactMapper : MapperBase<FacilityContact, DepositoContacto>
{
    /// <summary>
    /// BD → Domain: DepositoContacto → FacilityContact
    /// </summary>
    public override FacilityContact ToDomain(DepositoContacto dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        // Get AccountId from related Facility (Deposito has IdCuenta)
        var accountId = string.Empty;
        if (dbEntity.IdDepositoNavigation?.IdCuenta.HasValue == true)
        {
            accountId = dbEntity.IdDepositoNavigation.IdCuenta.Value.ToString();
        }

        return new FacilityContact
        {
            // IDs - Domain BaseEntity uses string
            Id = Guid.NewGuid().ToString(),
            AccountId = accountId,
            
            // Relations
            FacilityId = IdConversion.ToStringFromLong(dbEntity.IdDeposito),
            ContactId = dbEntity.IdContacto ?? string.Empty,
            RelationshipType = dbEntity.IdRelacion ?? string.Empty,
            
            // Properties
            Notes = dbEntity.Notas,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: FacilityContact → DepositoContacto
    /// </summary>
    public override DepositoContacto ToDatabase(FacilityContact domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new DepositoContacto
        {
            // IDs (composite key)
            IdDeposito = IdConversion.ToLongFromString(domainEntity.FacilityId),
            IdContacto = domainEntity.ContactId ?? string.Empty,
            IdRelacion = domainEntity.RelationshipType,
            
            // Properties
            Notas = domainEntity.Notes,
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

    public override void UpdateDatabase(FacilityContact domainEntity, DepositoContacto dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.IdRelacion = domainEntity.RelationshipType;
        dbEntity.Notas = domainEntity.Notes;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

