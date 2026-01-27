using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Supply (Domain/Inglés) ↔ Insumo (BD/Español)
/// Maps between clean domain entity and database entity
/// </summary>
public class SupplyMapper : MapperBase<Supply, Insumo>
{
    /// <summary>
    /// BD → Domain: Insumo → Supply
    /// </summary>
    public override Supply ToDomain(Insumo dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new Supply
        {
            // IDs - Domain BaseEntity uses string
            Id = GuidLongConverter.ToGuid(dbEntity.IdInsumo).ToString(),
            AccountId = string.Empty, // Insumo doesn't have AccountId directly, it's public/private
            
            // Basic Info
            Code = dbEntity.IdInsumo.ToString(), // Use ID as code if no code field exists
            Name = dbEntity.Nombre ?? string.Empty,
            Description = null, // Insumo doesn't have description field
            CategoryUnitId = dbEntity.IdCategoriaUnidad,
            
            // Properties
            IsPublic = dbEntity.Publico,
            
            // Parent Supply (hierarchical)
            ParentSupplyId = dbEntity.IdInsumoSuperior.HasValue 
                ? GuidLongConverter.ToGuid(dbEntity.IdInsumoSuperior.Value) 
                : null,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: Supply → Insumo
    /// </summary>
    public override Insumo ToDatabase(Supply domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new Insumo
        {
            // IDs
            IdInsumo = string.IsNullOrEmpty(domainEntity.Id) ? 0 : GuidLongConverter.ToLong(Guid.Parse(domainEntity.Id)),
            
            // Basic Info
            Nombre = domainEntity.Name,
            IdCategoriaUnidad = domainEntity.CategoryUnitId,
            
            // Properties
            Publico = domainEntity.IsPublic,
            
            // Parent Supply (hierarchical)
            IdInsumoSuperior = domainEntity.ParentSupplyId.HasValue 
                ? GuidLongConverter.ToLong(domainEntity.ParentSupplyId.Value) 
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

    public override void UpdateDatabase(Supply domainEntity, Insumo dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.IdCategoriaUnidad = domainEntity.CategoryUnitId;
        dbEntity.Publico = domainEntity.IsPublic;
        dbEntity.IdInsumoSuperior = domainEntity.ParentSupplyId.HasValue 
            ? GuidLongConverter.ToLong(domainEntity.ParentSupplyId.Value) 
            : null;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

