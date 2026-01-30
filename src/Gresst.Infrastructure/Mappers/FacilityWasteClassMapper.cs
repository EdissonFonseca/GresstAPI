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
        var accountId = string.Empty;
        if (dbEntity.IdDepositoNavigation?.IdCuenta.HasValue == true)
        {
            accountId = dbEntity.IdDepositoNavigation.IdCuenta.Value.ToString();
        }

        // Convert int IdTipoResiduo to string (domain ID)
        var wasteClassId = IdConversion.ToStringFromLong(dbEntity.IdTipoResiduo);

        return new FacilityWasteClass
        {
            // IDs - Domain BaseEntity uses string
            Id = Guid.NewGuid().ToString(),
            AccountId = accountId,
            
            // Relations
            FacilityId = IdConversion.ToStringFromLong(dbEntity.IdDeposito),
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

        // Convert string WasteClassId to int (domain ID to DB int)
        var wasteClassIdLong = IdConversion.ToLongFromString(domainEntity.WasteClassId);
        var wasteClassIdInt = (int)wasteClassIdLong; // Cast long to int

        return new DepositoTipoResiduo
        {
            // IDs (composite key)
            IdDeposito = IdConversion.ToLongFromString(domainEntity.FacilityId),
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

