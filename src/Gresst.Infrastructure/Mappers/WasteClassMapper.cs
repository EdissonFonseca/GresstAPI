using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using DomainWasteClass = Gresst.Domain.Entities.WasteClass;
using DbWasteClass = Gresst.Infrastructure.Data.Entities.TipoResiduo;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: WasteClass (Domain/Inglés) ↔ TipoResiduo (BD/Español)
/// </summary>
public class WasteClassMapper : MapperBase<DomainWasteClass, DbWasteClass>
{
    /// <summary>
    /// BD → Domain: TipoResiduo → WasteClass
    /// </summary>
    public override DomainWasteClass ToDomain(DbWasteClass dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new DomainWasteClass
        {
            // IDs - Conversión de int a Guid
            Id = dbEntity.IdTipoResiduo != 0 
                ? GuidLongConverter.ToGuid(dbEntity.IdTipoResiduo) 
                : Guid.NewGuid(),
            
            // Basic Info
            Code = dbEntity.IdTipoResiduo.ToString(), // Usar ID como código si no hay código específico
            Name = dbEntity.Nombre ?? string.Empty,
            Description = dbEntity.Descripcion,
            
            // Properties - No están directamente en BD, usar valores por defecto
            IsHazardous = false,
            RequiresSpecialHandling = false,
            PhysicalState = null,
            
            // Classification - No está directamente en BD
            ClassificationId = null,
            
            // Treatment - Se maneja a través de relación separada
            TreatmentId = null,
            
            // Audit fields
            AccountId = Guid.Empty, // TipoResiduo no tiene IdCuenta directo
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: WasteClass → TipoResiduo
    /// </summary>
    public override DbWasteClass ToDatabase(DomainWasteClass domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new DbWasteClass
        {
            // IDs - Conversión de Guid a int
            IdTipoResiduo = domainEntity.Id != Guid.Empty 
                ? (int)GuidLongConverter.ToLong(domainEntity.Id) 
                : 0,
            
            // Basic Info
            Nombre = domainEntity.Name,
            Descripcion = domainEntity.Description,
            
            // Properties
            Activo = domainEntity.IsActive,
            Publico = false, // Por defecto, se puede configurar si es necesario
            
            // Audit
            IdUsuarioCreacion = !string.IsNullOrEmpty(domainEntity.CreatedBy) 
                ? long.Parse(domainEntity.CreatedBy) 
                : 0,
            FechaCreacion = domainEntity.CreatedAt,
            IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
                ? long.Parse(domainEntity.UpdatedBy) 
                : null,
            FechaUltimaModificacion = domainEntity.UpdatedAt
        };
    }

    public override void UpdateDatabase(DomainWasteClass domainEntity, DbWasteClass dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Descripcion = domainEntity.Description;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

