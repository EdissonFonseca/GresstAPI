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
        var accountId = Guid.Empty;
        if (dbEntity.IdDepositoNavigation?.IdCuenta.HasValue == true)
        {
            accountId = GuidLongConverter.ToGuid(dbEntity.IdDepositoNavigation.IdCuenta.Value);
        }

        return new FacilityContact
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid(),
            AccountId = accountId,
            
            // Relations
            FacilityId = GuidLongConverter.ToGuid(dbEntity.IdDeposito),
            ContactId = GuidLongConverter.StringToGuid(dbEntity.IdContacto),
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
            IdDeposito = GuidLongConverter.ToLong(domainEntity.FacilityId),
            IdContacto = GuidLongConverter.GuidToString(domainEntity.ContactId),
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

