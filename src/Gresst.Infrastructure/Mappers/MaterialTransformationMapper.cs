using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: MaterialTransformation (Domain/Inglés) ↔ MaterialItem (BD/Español)
/// Maps between clean domain entity and database entity
/// Note: MaterialItem uses IdItem as source and IdMaterial as result
/// </summary>
public class MaterialTransformationMapper : MapperBase<MaterialTransformation, MaterialItem>
{
    /// <summary>
    /// BD → Domain: MaterialItem → MaterialTransformation
    /// </summary>
    public override MaterialTransformation ToDomain(MaterialItem dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        // Get AccountId from related Material (Material doesn't have IdCuenta directly, use default)
        var accountId = string.Empty;

        return new MaterialTransformation
        {
            // IDs - Domain BaseEntity uses string
            Id = Guid.NewGuid().ToString(),
            AccountId = accountId,
            
            // Relations
            // MaterialItem: IdItem is source, IdMaterial is result
            SourceMaterialId = GuidLongConverter.ToGuid(dbEntity.IdItem),
            ResultMaterialId = GuidLongConverter.ToGuid(dbEntity.IdMaterial),
            RelationshipType = dbEntity.IdRelacion ?? string.Empty,
            
            // Conversion factors
            Percentage = dbEntity.Porcentaje,
            Quantity = dbEntity.Cantidad,
            Weight = dbEntity.Peso,
            Volume = dbEntity.Volumen,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = true // MaterialItem doesn't have Activo field, assume true
        };
    }

    /// <summary>
    /// Domain → BD: MaterialTransformation → MaterialItem
    /// </summary>
    public override MaterialItem ToDatabase(MaterialTransformation domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new MaterialItem
        {
            // IDs (composite key)
            // MaterialItem: IdItem is source, IdMaterial is result
            IdItem = GuidLongConverter.ToLong(domainEntity.SourceMaterialId),
            IdMaterial = GuidLongConverter.ToLong(domainEntity.ResultMaterialId),
            IdRelacion = domainEntity.RelationshipType,
            
            // Conversion factors
            Porcentaje = domainEntity.Percentage,
            Cantidad = domainEntity.Quantity,
            Peso = domainEntity.Weight,
            Volumen = domainEntity.Volume,
            
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

    public override void UpdateDatabase(MaterialTransformation domainEntity, MaterialItem dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.IdRelacion = domainEntity.RelationshipType;
        
        // Conversion factors
        dbEntity.Porcentaje = domainEntity.Percentage;
        dbEntity.Cantidad = domainEntity.Quantity;
        dbEntity.Peso = domainEntity.Weight;
        dbEntity.Volumen = domainEntity.Volume;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

