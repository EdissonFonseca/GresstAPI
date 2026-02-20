using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Facility (Domain/Inglés) ↔ Deposito (BD/Español)
/// </summary>
public class FacilityMapper : MapperBase<Facility, Deposito>
{
    private readonly ICurrentUserService _currentUserService;

    public FacilityMapper(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

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
            
            // Basic Info
            Name = dbEntity.Nombre ?? string.Empty,
            Description = dbEntity.Notas,
            //Type = DetermineFacilityType(dbEntity),
            
            // Location
            Address = dbEntity.Direccion,
            //Latitude = dbEntity.Ubicacion.GetLatitude(),
            //Longitude = dbEntity.Ubicacion.GetLongitude(),
            
            // Capacity
            MaxCapacity = dbEntity.Peso,
            CapacityUnit = "kg",
            CurrentCapacity = dbEntity.Cantidad,
            
            ParentId = dbEntity.IdSuperior.HasValue ? dbEntity.IdSuperior.Value.ToString() : null,
            
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
        };
    }

    /// <summary>
    /// Domain → BD: Facility → Deposito
    /// </summary>
    public override Deposito ToDatabase(Facility domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        var accountId = _currentUserService.GetCurrentAccountId();

        return new Deposito
        {
            // IDs - Domain Id/AccountId are string, BD uses long
            IdDeposito = long.TryParse(domainEntity.Id, out var idDeposito) ? idDeposito : 0,
            IdCuenta = long.TryParse(accountId, out var idCuenta) ? idCuenta : 0,
            
            // Basic Info
            Nombre = domainEntity.Name,
            Notas = domainEntity.Description,
            
            // Location
            Direccion = domainEntity.Address,
            //Ubicacion = NetTopologySuiteExtensions.CreatePoint(domainEntity.Latitude, domainEntity.Longitude),
            
            // Owner - DB IdPersona is string
            //IdPersona = string.IsNullOrEmpty(domainEntity.PersonId) ? null : domainEntity.PersonId,
            
            // Capacity
            Peso = domainEntity.MaxCapacity,
            Cantidad = domainEntity.CurrentCapacity,
            
            // Parent Facility - Domain ParentFacilityId is string
            //IdSuperior = !string.IsNullOrEmpty(domainEntity.ParentId) 
            //    ? domainEntity.ParentId.ToString() 
            //    : null,
            
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
        //dbEntity.Ubicacion = NetTopologySuiteExtensions.CreatePoint(domainEntity.Latitude, domainEntity.Longitude);
        
        //// Capabilities
        //dbEntity.Acopio = domainEntity.CanCollect;
        //dbEntity.Almacenamiento = domainEntity.CanStore;
        //dbEntity.Disposicion = domainEntity.CanDispose;
        //dbEntity.Entrega = domainEntity.CanDeliver;
        //dbEntity.Recepcion = domainEntity.CanReceive;
        //dbEntity.Tratamiento = domainEntity.CanTreat;
        
        //// Capacity
        //dbEntity.Peso = domainEntity.MaxCapacity;
        //dbEntity.Cantidad = domainEntity.CurrentCapacity;
        
        //// Parent Facility - Domain ParentFacilityId is string
        //dbEntity.IdSuperior = !string.IsNullOrEmpty(domainEntity.ParentFacilityId) 
        //    ? IdConversion.ToLongFromString(domainEntity.ParentFacilityId) 
        //    : null;
        
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

