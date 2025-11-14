using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: FacilityWasteClass (Domain/Inglés) ↔ DepositoTipoResiduo (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class FacilityWasteClassMapper : MapperBase<FacilityWasteClass, DepositoTipoResiduo>
{
    /// <summary>
    /// BD → Domain: DepositoTipoResiduo → FacilityWasteClass
    /// </summary>
    public override FacilityWasteClass ToDomain(DepositoTipoResiduo dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        // Get AccountId from related Facility (Deposito has IdCuenta)
        var accountId = Guid.Empty;
        if (dbEntity.IdDepositoNavigation?.IdCuenta.HasValue == true)
        {
            accountId = GuidLongConverter.ToGuid(dbEntity.IdDepositoNavigation.IdCuenta.Value);
        }

        // Convert int IdTipoResiduo to Guid (using GuidLongConverter pattern)
        var wasteClassId = GuidLongConverter.ToGuid(dbEntity.IdTipoResiduo);

        return new FacilityWasteClass
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid(),
            AccountId = accountId,
            
            // Relations
            FacilityId = GuidLongConverter.ToGuid(dbEntity.IdDeposito),
            WasteClassId = wasteClassId,
            RelationshipType = dbEntity.IdRelacion ?? string.Empty,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = true // DepositoTipoResiduo doesn't have Activo field, assume true
        };
    }

    /// <summary>
    /// Domain → BD: FacilityWasteClass → DepositoTipoResiduo
    /// </summary>
    public override DepositoTipoResiduo ToDatabase(FacilityWasteClass domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        // Convert Guid WasteClassId to int (using GuidLongConverter pattern, then cast to int)
        var wasteClassIdLong = GuidLongConverter.ToLong(domainEntity.WasteClassId);
        var wasteClassIdInt = (int)wasteClassIdLong; // Cast long to int

        return new DepositoTipoResiduo
        {
            // IDs (composite key)
            IdDeposito = GuidLongConverter.ToLong(domainEntity.FacilityId),
            IdTipoResiduo = wasteClassIdInt,
            IdRelacion = domainEntity.RelationshipType,
            
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

    public override void UpdateDatabase(FacilityWasteClass domainEntity, DepositoTipoResiduo dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.IdRelacion = domainEntity.RelationshipType;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

