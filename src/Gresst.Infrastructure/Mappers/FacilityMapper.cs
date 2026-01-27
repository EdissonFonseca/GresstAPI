using Gresst.Domain.Entities;
using Gresst.Infrastructure.Data.Entities;
using NetTopologySuite.Geometries;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Facility (Domain/Inglés) ↔ Deposito (BD/Español)
/// </summary>
public class FacilityMapper : MapperBase<Facility, Deposito>
{
    /// <summary>
    /// BD → Domain: Deposito → Facility
    /// </summary>
    public override Facility ToDomain(Deposito dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new Facility
        {
            // IDs - Domain uses string for BaseEntity.Id/AccountId
            Id = dbEntity.IdDeposito.ToString(),
            AccountId = dbEntity.IdCuenta?.ToString() ?? string.Empty,
            
            // Basic Info
            Code = dbEntity.IdDeposito.ToString(),
            Name = dbEntity.Nombre ?? string.Empty,
            Description = dbEntity.Notas,
            FacilityType = DetermineFacilityType(dbEntity),
            
            // Location
            Address = dbEntity.Direccion,
            Latitude = dbEntity.Ubicacion.GetLatitude(),
            Longitude = dbEntity.Ubicacion.GetLongitude(),
            
            // Owner - Conversión de string a Guid
            PersonId = !string.IsNullOrEmpty(dbEntity.IdPersona) 
                ? new Guid(dbEntity.IdPersona.PadLeft(32, '0')) 
                : Guid.Empty,
            
            // Capabilities - Mapeo directo de booleanos
            CanCollect = dbEntity.Acopio,
            CanStore = dbEntity.Almacenamiento,
            CanDispose = dbEntity.Disposicion,
            CanDeliver = dbEntity.Entrega,
            CanReceive = dbEntity.Recepcion,
            CanTreat = dbEntity.Tratamiento,
            
            // Capacity
            MaxCapacity = dbEntity.Peso,
            CapacityUnit = "kg",
            CurrentCapacity = dbEntity.Cantidad,
            
            // Virtual and Parent
            IsVirtual = false, // Deposito doesn't have IsVirtual field, default to false
            ParentFacilityId = dbEntity.IdSuperior.HasValue 
                ? new Guid(dbEntity.IdSuperior.Value.ToString().PadLeft(32, '0')) 
                : null,
            // Map ParentFacility navigation if loaded (for eager loading)
            ParentFacility = dbEntity.IdSuperiorNavigation != null 
                ? MapParentFacility(dbEntity.IdSuperiorNavigation) 
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
    /// Helper method to map parent facility (lightweight mapping, no recursive navigation)
    /// </summary>
    private Facility? MapParentFacility(Deposito parentDbEntity)
    {
        if (parentDbEntity == null) return null;
        
        return new Facility
        {
            Id = parentDbEntity.IdDeposito.ToString(),
            Name = parentDbEntity.Nombre ?? string.Empty,
            Code = parentDbEntity.IdDeposito.ToString(),
            // Only map essential fields to avoid deep recursion
            // Other fields will be null/default
        };
    }

    /// <summary>
    /// Domain → BD: Facility → Deposito
    /// </summary>
    public override Deposito ToDatabase(Facility domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new Deposito
        {
            // IDs - Domain Id/AccountId are string, BD uses long
            IdDeposito = string.IsNullOrEmpty(domainEntity.Id) ? 0 : long.Parse(domainEntity.Id),
            IdCuenta = string.IsNullOrEmpty(domainEntity.AccountId) ? null : long.Parse(domainEntity.AccountId),
            
            // Basic Info
            Nombre = domainEntity.Name,
            Notas = domainEntity.Description,
            Referencia = domainEntity.Code,
            
            // Location
            Direccion = domainEntity.Address,
            Ubicacion = NetTopologySuiteExtensions.CreatePoint(domainEntity.Latitude, domainEntity.Longitude),
            
            // Owner
            IdPersona = domainEntity.PersonId != Guid.Empty 
                ? domainEntity.PersonId.ToString().Replace("-", "").Substring(0, 40) 
                : null,
            
            // Capabilities
            Acopio = domainEntity.CanCollect,
            Almacenamiento = domainEntity.CanStore,
            Disposicion = domainEntity.CanDispose,
            Entrega = domainEntity.CanDeliver,
            Recepcion = domainEntity.CanReceive,
            Tratamiento = domainEntity.CanTreat,
            
            // Capacity
            Peso = domainEntity.MaxCapacity,
            Cantidad = domainEntity.CurrentCapacity,
            
            // Parent Facility
            IdSuperior = domainEntity.ParentFacilityId.HasValue 
                ? long.Parse(domainEntity.ParentFacilityId.Value.ToString().Replace("-", "").Substring(0, 18)) 
                : null,
            
            // Audit
            FechaCreacion = domainEntity.CreatedAt,
            FechaUltimaModificacion = domainEntity.UpdatedAt,
            IdUsuarioCreacion = !string.IsNullOrEmpty(domainEntity.CreatedBy) 
                ? long.Parse(domainEntity.CreatedBy) 
                : 0,
            IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
                ? long.Parse(domainEntity.UpdatedBy) 
                : null,
            Activo = domainEntity.IsActive
        };
    }

    /// <summary>
    /// Update existing BD entity with Domain values
    /// </summary>
    public override void UpdateDatabase(Facility domainEntity, Deposito dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Solo actualizar campos modificables (no IDs, no fechas de creación)
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Notas = domainEntity.Description;
        dbEntity.Direccion = domainEntity.Address;
        dbEntity.Ubicacion = NetTopologySuiteExtensions.CreatePoint(domainEntity.Latitude, domainEntity.Longitude);
        
        // Capabilities
        dbEntity.Acopio = domainEntity.CanCollect;
        dbEntity.Almacenamiento = domainEntity.CanStore;
        dbEntity.Disposicion = domainEntity.CanDispose;
        dbEntity.Entrega = domainEntity.CanDeliver;
        dbEntity.Recepcion = domainEntity.CanReceive;
        dbEntity.Tratamiento = domainEntity.CanTreat;
        
        // Capacity
        dbEntity.Peso = domainEntity.MaxCapacity;
        dbEntity.Cantidad = domainEntity.CurrentCapacity;
        
        // Parent Facility
        dbEntity.IdSuperior = domainEntity.ParentFacilityId.HasValue 
            ? long.Parse(domainEntity.ParentFacilityId.Value.ToString().Replace("-", "").Substring(0, 18)) 
            : null;
        
        // Note: IsVirtual is not stored in Deposito table, it's a domain concept
        // Virtual facilities should be identified by a specific FacilityType or naming convention
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
        dbEntity.Activo = domainEntity.IsActive;
    }

    // Helper method to determine facility type based on capabilities
    private string DetermineFacilityType(Deposito dbEntity)
    {
        if (dbEntity.Tratamiento) return "TreatmentPlant";
        if (dbEntity.Disposicion) return "DisposalSite";
        if (dbEntity.Almacenamiento) return "StorageFacility";
        if (dbEntity.Acopio && dbEntity.Entrega) return "TransferStation";
        return "Facility";
    }
}

