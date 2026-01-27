using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: PersonMaterial (Domain/Inglés) ↔ PersonaMaterial (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class PersonMaterialMapper : MapperBase<PersonMaterial, PersonaMaterial>
{
    /// <summary>
    /// BD → Domain: PersonaMaterial → PersonMaterial
    /// </summary>
    public override PersonMaterial ToDomain(PersonaMaterial dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new PersonMaterial
        {
            // IDs
            Id = Guid.NewGuid().ToString(), // PersonMaterial doesn't have a single ID in BD, it's a composite key
            AccountId = dbEntity.IdCuenta.ToString(),
            
            // Relations
            PersonId = GuidStringConverter.ToGuid(dbEntity.IdPersona),
            MaterialId = GuidLongConverter.ToGuid(dbEntity.IdMaterial),
            PackagingId = dbEntity.IdEmbalaje.HasValue 
                ? GuidLongConverter.ToGuid(dbEntity.IdEmbalaje.Value) 
                : null,
            
            // Properties
            Name = dbEntity.Nombre,
            ServicePrice = dbEntity.PrecioServicio,
            PurchasePrice = dbEntity.PrecioCompra,
            SalePrice = dbEntity.PrecioVenta,
            Weight = dbEntity.Peso,
            Volume = dbEntity.Volumen,
            EmissionCompensationFactor = dbEntity.FactorCompensacionEmision,
            Reference = dbEntity.Referencia,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: PersonMaterial → PersonaMaterial
    /// </summary>
    public override PersonaMaterial ToDatabase(PersonMaterial domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new PersonaMaterial
        {
            // IDs (composite key)
            IdPersona = GuidStringConverter.ToString(domainEntity.PersonId),
            IdMaterial = GuidLongConverter.ToLong(domainEntity.MaterialId),
            IdCuenta = long.TryParse(domainEntity.AccountId, out var pmAc) ? pmAc : 0,
            
            // Properties
            Nombre = domainEntity.Name,
            PrecioServicio = domainEntity.ServicePrice,
            PrecioCompra = domainEntity.PurchasePrice,
            PrecioVenta = domainEntity.SalePrice,
            Peso = domainEntity.Weight,
            Volumen = domainEntity.Volume,
            FactorCompensacionEmision = domainEntity.EmissionCompensationFactor,
            Referencia = domainEntity.Reference,
            IdEmbalaje = domainEntity.PackagingId.HasValue 
                ? GuidLongConverter.ToLong(domainEntity.PackagingId.Value) 
                : null,
            
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

    public override void UpdateDatabase(PersonMaterial domainEntity, PersonaMaterial dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.PrecioServicio = domainEntity.ServicePrice;
        dbEntity.PrecioCompra = domainEntity.PurchasePrice;
        dbEntity.PrecioVenta = domainEntity.SalePrice;
        dbEntity.Peso = domainEntity.Weight;
        dbEntity.Volumen = domainEntity.Volume;
        dbEntity.FactorCompensacionEmision = domainEntity.EmissionCompensationFactor;
        dbEntity.Referencia = domainEntity.Reference;
        dbEntity.IdEmbalaje = domainEntity.PackagingId.HasValue 
            ? GuidLongConverter.ToLong(domainEntity.PackagingId.Value) 
            : null;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

