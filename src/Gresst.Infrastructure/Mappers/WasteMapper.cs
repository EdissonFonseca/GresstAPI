using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Waste (Domain/Inglés) ↔ Residuo (BD/Español)
/// </summary>
public class WasteMapper : MapperBase<Waste, Residuo>
{
    /// <summary>
    /// BD → Domain: Residuo → Waste
    /// </summary>
    public override Waste ToDomain(Residuo dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new Waste
        {
            // IDs - Domain BaseEntity uses string
            Id = IdConversion.ToStringFromLong(dbEntity.IdResiduo),
            AccountId = string.Empty, // Se obtiene del usuario actual en DbContext
            
            // Basic Info
            Code = dbEntity.Referencia ?? dbEntity.IdResiduo.ToString(),
            Description = dbEntity.Descripcion,
            
            // Waste Type - IdMaterial en BD es como WasteClassId
            WasteClassId = IdConversion.ToStringFromLong(dbEntity.IdMaterial),
            
            // Quantity - Asumiendo que Residuo no tiene cantidad directa, se obtiene de gestiones
            Quantity = 0, // Se calculará desde las gestiones
            Unit = UnitOfMeasure.Kilogram, // Default
            
            // Status
            Status = MapStatus(dbEntity.IdEstado),
            
            // Generator - Asumiendo que el propietario es el generador inicialmente
            GeneratorId = dbEntity.IdPropietario ?? string.Empty,
            GeneratedAt = dbEntity.FechaIngreso ?? dbEntity.FechaCreacion,
            
            // Current Owner - DB IdPropietario is string
            CurrentOwnerId = dbEntity.IdPropietario,
            
            // Properties
            IsHazardous = false, // Determinar desde TipoResiduo/Material
            IsAvailableInBank = false, // No hay campo directo en BD
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = true // Residuo no tiene campo Activo, asumimos true si existe
        };
    }

    /// <summary>
    /// Domain → BD: Waste → Residuo
    /// </summary>
    public override Residuo ToDatabase(Waste domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new Residuo
        {
            // IDs
            IdResiduo = IdConversion.ToLongFromString(domainEntity.Id),
            IdMaterial = IdConversion.ToLongFromString(domainEntity.WasteClassId),
            
            // Owner - DB IdPropietario is string
            IdPropietario = domainEntity.CurrentOwnerId,
            
            // Info
            Descripcion = domainEntity.Description,
            Referencia = domainEntity.Code,
            Soporte = null, // JSON de archivos si se necesita
            
            // Status
            IdEstado = MapStatusToDb(domainEntity.Status),
            
            // Dates
            FechaIngreso = domainEntity.GeneratedAt,
            FechaCreacion = domainEntity.CreatedAt,
            FechaUltimaModificacion = domainEntity.UpdatedAt,
            
            // Audit
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
    public override void UpdateDatabase(Waste domainEntity, Residuo dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Actualizar campos modificables
        dbEntity.Descripcion = domainEntity.Description;
        dbEntity.IdEstado = MapStatusToDb(domainEntity.Status);
        dbEntity.IdPropietario = domainEntity.CurrentOwnerId;
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }

    // Helper methods - Status mapping
    private WasteStatus MapStatus(string dbStatus)
    {
        return dbStatus?.ToUpper() switch
        {
            "G" => WasteStatus.Generated,
            "T" => WasteStatus.InTransit,
            "A" => WasteStatus.Stored,
            "D" => WasteStatus.Disposed,
            "R" => WasteStatus.Transformed,
            "E" => WasteStatus.Delivered,
            "V" => WasteStatus.Sold,
            _ => WasteStatus.Generated
        };
    }

    private string MapStatusToDb(WasteStatus status)
    {
        return status switch
        {
            WasteStatus.Generated => "G",
            WasteStatus.InTransit => "T",
            WasteStatus.Stored => "A",
            WasteStatus.InTreatment => "T",
            WasteStatus.Disposed => "D",
            WasteStatus.Transformed => "R",
            WasteStatus.Delivered => "E",
            WasteStatus.Sold => "V",
            WasteStatus.Reused => "R",
            _ => "G"
        };
    }

}

