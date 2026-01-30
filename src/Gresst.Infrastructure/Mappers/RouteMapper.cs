using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using DomainRoute = Gresst.Domain.Entities.Route;
using DbRoute = Gresst.Infrastructure.Data.Entities.Rutum;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Route (Domain/Inglés) ↔ Rutum (BD/Español)
/// Note: In BD, IdVehiculo is mandatory, but in Domain VehicleId is optional.
/// When VehicleId is null, we use a placeholder value "SIN-VEHICULO" that can be updated later.
/// </summary>
public class RouteMapper : MapperBase<DomainRoute, DbRoute>
{

    /// <summary>
    /// BD → Domain: Rutum → Route
    /// </summary>
    public override DomainRoute ToDomain(DbRoute dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        // IdVehiculo in BD is the LicensePlate (string); Domain VehicleId is string? (vehicle ID or null)
        string? vehicleId = null;
        if (!string.IsNullOrEmpty(dbEntity.IdVehiculo) && 
            !dbEntity.IdVehiculo.Equals("SIN-VEHICULO", StringComparison.OrdinalIgnoreCase))
        {
            // Service layer may resolve LicensePlate to vehicle ID; for mapper we keep null
            vehicleId = null;
        }

        return new DomainRoute
        {
            // IDs - Domain BaseEntity uses string
            Id = dbEntity.IdRuta != 0 
                ? IdConversion.ToStringFromLong(dbEntity.IdRuta) 
                : string.Empty,
            
            // Basic Info
            Code = dbEntity.IdRuta.ToString(), // Usar ID como código si no hay código específico
            Name = dbEntity.Nombre ?? string.Empty,
            Description = null, // No está en BD directamente
            
            // Route Type - No está directamente en BD, usar valor por defecto
            RouteType = "Collection", // Valor por defecto
            
            // Assignment - DB IdResponsable is string
            VehicleId = vehicleId,
            DriverId = dbEntity.IdResponsable,
            
            // Scheduling
            Schedule = dbEntity.Recurrencia, // JSON: Days of week, frequency
            EstimatedDuration = dbEntity.Duracion, // in hours
            EstimatedDistance = null, // No está en BD
            
            // Status
            IsActive = dbEntity.Activo,
            
            // Audit fields
            AccountId = dbEntity.IdCuenta.ToString(),
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString()
        };
    }

    /// <summary>
    /// Domain → BD: Route → Rutum
    /// Note: If VehicleId is null, we use "SIN-VEHICULO" as placeholder
    /// </summary>
    public override DbRoute ToDatabase(DomainRoute domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        // VehicleId (string?) - BD IdVehiculo is LicensePlate (string, mandatory)
        string idVehiculo;
        if (!string.IsNullOrEmpty(domainEntity.VehicleId) && 
            domainEntity.Vehicle != null && !string.IsNullOrEmpty(domainEntity.Vehicle.LicensePlate))
        {
            idVehiculo = domainEntity.Vehicle.LicensePlate;
        }
        else
        {
            idVehiculo = "SIN-VEHICULO";
        }

        return new DbRoute
        {
            // IDs - Domain Id is string
            IdRuta = IdConversion.ToLongFromString(domainEntity.Id),
            
            // Basic Info
            Nombre = domainEntity.Name,
            
            // Assignment - DB IdResponsable is string
            IdVehiculo = idVehiculo,
            IdResponsable = domainEntity.DriverId ?? string.Empty,
            
            // Scheduling
            Recurrencia = domainEntity.Schedule ?? string.Empty,
            FechaInicio = null, // No está directamente en dominio
            FechaFin = null, // No está directamente en dominio
            DiaCompleto = false, // No está directamente en dominio
            HoraInicio = null, // No está directamente en dominio
            HoraFin = null, // No está directamente en dominio
            Duracion = domainEntity.EstimatedDuration,
            
            // Properties
            Activo = domainEntity.IsActive,
            IdCuenta = long.TryParse(domainEntity.AccountId, out var accountLong) ? accountLong : 0,
            
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

    public override void UpdateDatabase(DomainRoute domainEntity, DbRoute dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Nombre = domainEntity.Name;
        dbEntity.Recurrencia = domainEntity.Schedule ?? string.Empty;
        dbEntity.Duracion = domainEntity.EstimatedDuration;
        dbEntity.Activo = domainEntity.IsActive;
        
        // Update VehicleId if provided
        if (!string.IsNullOrEmpty(domainEntity.VehicleId) &&
            domainEntity.Vehicle != null && !string.IsNullOrEmpty(domainEntity.Vehicle.LicensePlate))
        {
            dbEntity.IdVehiculo = domainEntity.Vehicle.LicensePlate;
        }
        else if (string.IsNullOrEmpty(domainEntity.VehicleId))
        {
            dbEntity.IdVehiculo = "SIN-VEHICULO";
        }
        
        // Update DriverId if provided - Domain DriverId is string?
        if (!string.IsNullOrEmpty(domainEntity.DriverId))
        {
            dbEntity.IdResponsable = domainEntity.DriverId;
        }
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

