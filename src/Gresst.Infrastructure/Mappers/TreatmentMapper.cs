using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using DomainTreatment = Gresst.Domain.Entities.Treatment;
using DbTreatment = Gresst.Infrastructure.Data.Entities.Tratamiento;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Treatment (Domain/Inglés) ↔ Tratamiento (BD/Español)
/// </summary>
public class TreatmentMapper : MapperBase<DomainTreatment, DbTreatment>
{
    /// <summary>
    /// BD → Domain: Tratamiento → Treatment
    /// </summary>
    public override DomainTreatment ToDomain(DbTreatment dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new DomainTreatment
        {
            // IDs - Conversión de long a Guid
            Id = dbEntity.IdTratamiento != 0 
                ? GuidLongConverter.ToGuid(dbEntity.IdTratamiento) 
                : Guid.NewGuid(),
            
            // Basic Info
            Code = dbEntity.IdTratamiento.ToString(), // Usar ID como código si no hay código específico
            Name = dbEntity.Nombre ?? string.Empty,
            Description = dbEntity.Descripcion,
            
            // Category
            Category = dbEntity.IdCategoria ?? string.Empty,
            
            // Service
            ServiceId = GuidLongConverter.ToGuid(dbEntity.IdServicio),
            
            // Process details - No están directamente en BD, usar valores por defecto
            ProcessDescription = null,
            EstimatedDuration = null,
            
            // Applicable waste classes - No está directamente en BD
            ApplicableWasteClasses = null,
            
            // Results
            ProducesNewWaste = dbEntity.Aprovechamiento,
            ResultingWasteClasses = null,
            
            // Audit fields
            AccountId = Guid.Empty, // Tratamiento no tiene IdCuenta directo
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = dbEntity.Activo
        };
    }

    /// <summary>
    /// Domain → BD: Treatment → Tratamiento
    /// </summary>
    public override DbTreatment ToDatabase(DomainTreatment domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new DbTreatment
        {
            // IDs - Conversión de Guid a long
            IdTratamiento = domainEntity.Id != Guid.Empty 
                ? GuidLongConverter.ToLong(domainEntity.Id) 
                : 0,
            
            // Basic Info
            Nombre = domainEntity.Name,
            Descripcion = domainEntity.Description,
            
            // Category
            IdCategoria = domainEntity.Category,
            
            // Service
            IdServicio = GuidLongConverter.ToLong(domainEntity.ServiceId),
            
            // Results
            Aprovechamiento = domainEntity.ProducesNewWaste,
            
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

    public override void UpdateDatabase(DomainTreatment domainEntity, DbTreatment dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Descripcion = domainEntity.Description;
        dbEntity.IdCategoria = domainEntity.Category;
        dbEntity.IdServicio = GuidLongConverter.ToLong(domainEntity.ServiceId);
        dbEntity.Aprovechamiento = domainEntity.ProducesNewWaste;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

