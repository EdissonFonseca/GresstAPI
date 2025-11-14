using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: FacilityMaterial (Domain/Inglés) ↔ PersonaMaterialDeposito (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class FacilityMaterialMapper : MapperBase<FacilityMaterial, PersonaMaterialDeposito>
{
    /// <summary>
    /// BD → Domain: PersonaMaterialDeposito → FacilityMaterial
    /// </summary>
    public override FacilityMaterial ToDomain(PersonaMaterialDeposito dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new FacilityMaterial
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid(),
            AccountId = GuidLongConverter.ToGuid(dbEntity.IdCuenta),
            
            // Relations
            PersonId = GuidLongConverter.StringToGuid(dbEntity.IdPersona),
            MaterialId = GuidLongConverter.ToGuid(dbEntity.IdMaterial),
            FacilityId = GuidLongConverter.ToGuid(dbEntity.IdDeposito),
            
            // Properties
            ServicePrice = dbEntity.PrecioServicio,
            PurchasePrice = dbEntity.PrecioCompra,
            Weight = dbEntity.Peso,
            Volume = dbEntity.Volumen,
            EmissionCompensationFactor = dbEntity.FactorCompensacionEmision,
            IsHandled = dbEntity.Activo,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: FacilityMaterial → PersonaMaterialDeposito
    /// </summary>
    public override PersonaMaterialDeposito ToDatabase(FacilityMaterial domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaMaterialDeposito
        {
            // IDs (composite key)
            IdPersona = GuidLongConverter.GuidToString(domainEntity.PersonId),
            IdMaterial = GuidLongConverter.ToLong(domainEntity.MaterialId),
            IdDeposito = GuidLongConverter.ToLong(domainEntity.FacilityId),
            IdCuenta = GuidLongConverter.ToLong(domainEntity.AccountId),
            
            // Properties
            PrecioServicio = domainEntity.ServicePrice,
            PrecioCompra = domainEntity.PurchasePrice,
            Peso = domainEntity.Weight,
            Volumen = domainEntity.Volume,
            FactorCompensacionEmision = domainEntity.EmissionCompensationFactor,
            Activo = domainEntity.IsHandled,
            
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

    public override void UpdateDatabase(FacilityMaterial domainEntity, PersonaMaterialDeposito dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.PrecioServicio = domainEntity.ServicePrice;
        dbEntity.PrecioCompra = domainEntity.PurchasePrice;
        dbEntity.Peso = domainEntity.Weight;
        dbEntity.Volumen = domainEntity.Volume;
        dbEntity.FactorCompensacionEmision = domainEntity.EmissionCompensationFactor;
        dbEntity.Activo = domainEntity.IsHandled;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

