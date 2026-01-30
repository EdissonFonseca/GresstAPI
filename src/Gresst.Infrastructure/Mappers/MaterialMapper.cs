using Gresst.Infrastructure.Common;
using DomainMaterial = Gresst.Domain.Entities.Material;
using DbMaterial = Gresst.Infrastructure.Data.Entities.Material;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Material (Domain/Inglés) ↔ Material (BD/Español)
/// </summary>
public class MaterialMapper : MapperBase<DomainMaterial, DbMaterial>
{
    /// <summary>
    /// BD → Domain: Material (BD) → Material (Domain)
    /// </summary>
    public override DomainMaterial ToDomain(DbMaterial dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new DomainMaterial
        {
            // IDs - Domain uses string for BaseEntity.Id/AccountId
            Id = IdConversion.ToStringFromLong(dbEntity.IdMaterial),
            AccountId = string.Empty,
            
            // Basic Info
            Code = dbEntity.Referencia ?? dbEntity.IdMaterial.ToString(),
            Name = dbEntity.Nombre ?? string.Empty,
            Description = dbEntity.Descripcion,
            
            // Properties - Mapeo de campos de BD
            // IsRecyclable e IsHazardous no están directamente en BD, 
            // se pueden inferir de Aprovechable o dejarlos como false
            IsRecyclable = dbEntity.Aprovechable, // Aprovechable sugiere reciclable
            IsHazardous = false, // Por defecto, se puede inferir de otros campos si es necesario
            Category = null, // No está en BD directamente
            
            // Waste Type relationship
            WasteClassId = dbEntity.IdTipoResiduo.HasValue 
                ? IdConversion.ToStringFromLong(dbEntity.IdTipoResiduo.Value) 
                : null,
            
            // Audit fields
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: Material (Domain) → Material (BD)
    /// </summary>
    public override DbMaterial ToDatabase(DomainMaterial domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new DbMaterial
        {
            // IDs - Domain Id is string, BD uses long
            IdMaterial = IdConversion.ToLongFromString(domainEntity.Id),
            
            // Basic Info
            Nombre = domainEntity.Name,
            Descripcion = domainEntity.Description,
            Referencia = domainEntity.Code,
            
            // Waste Type relationship - Domain WasteClassId is string?
            IdTipoResiduo = !string.IsNullOrEmpty(domainEntity.WasteClassId) 
                ? (int?)IdConversion.ToLongFromString(domainEntity.WasteClassId) 
                : null,
            
            // Properties - Mapeo a campos de BD
            Aprovechable = domainEntity.IsRecyclable,
            
            // Defaults
            Activo = domainEntity.IsActive,
            Publico = true, // Por defecto público
            Medicion = "P", // Por defecto Peso
            
            // Audit
            FechaCreacion = domainEntity.CreatedAt,
            FechaUltimaModificacion = domainEntity.UpdatedAt,
            IdUsuarioCreacion = !string.IsNullOrEmpty(domainEntity.CreatedBy) 
                ? long.Parse(domainEntity.CreatedBy) 
                : 0,
            IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
                ? long.Parse(domainEntity.UpdatedBy) 
                : null
        };
    }

    /// <summary>
    /// Update existing BD entity with Domain values
    /// </summary>
    public override void UpdateDatabase(DomainMaterial domainEntity, DbMaterial dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Solo actualizar campos modificables (no IDs, no fechas de creación)
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Descripcion = domainEntity.Description;
        dbEntity.Referencia = domainEntity.Code;
        
        // Waste Type - Domain WasteClassId is string?
        dbEntity.IdTipoResiduo = !string.IsNullOrEmpty(domainEntity.WasteClassId) 
            ? (int?)IdConversion.ToLongFromString(domainEntity.WasteClassId) 
            : null;
        
        // Properties
        dbEntity.Aprovechable = domainEntity.IsRecyclable;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
        dbEntity.Activo = domainEntity.IsActive;
    }
}

