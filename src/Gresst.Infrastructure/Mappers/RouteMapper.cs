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

        // Convert IdVehiculo (string) to VehicleId (Guid?)
        // If IdVehiculo is a placeholder, set VehicleId to null
        Guid? vehicleId = null;
        if (!string.IsNullOrEmpty(dbEntity.IdVehiculo) && 
            !dbEntity.IdVehiculo.Equals("SIN-VEHICULO", StringComparison.OrdinalIgnoreCase))
        {
            // IdVehiculo in BD is the LicensePlate (string), not a Guid
            // We'll need to look up the vehicle by LicensePlate in the service layer
            // For now, set to null - the service will handle the lookup
            vehicleId = null;
        }

        return new DomainRoute
        {
            // IDs - Domain BaseEntity uses string
            Id = dbEntity.IdRuta != 0 
                ? GuidLongConverter.ToGuid(dbEntity.IdRuta).ToString() 
                : string.Empty,
            
            // Basic Info
            Code = dbEntity.IdRuta.ToString(), // Usar ID como código si no hay código específico
            Name = dbEntity.Nombre ?? string.Empty,
            Description = null, // No está en BD directamente
            
            // Route Type - No está directamente en BD, usar valor por defecto
            RouteType = "Collection", // Valor por defecto
            
            // Assignment
            VehicleId = vehicleId,
            DriverId = GuidStringConverter.ToGuid(dbEntity.IdResponsable),
            
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

        // Convert VehicleId (Guid?) to IdVehiculo (string, mandatory)
        // If VehicleId is null, use placeholder "SIN-VEHICULO"
        // Note: The repository will handle the actual lookup of LicensePlate from VehicleId
        string idVehiculo;
        if (domainEntity.VehicleId.HasValue && domainEntity.VehicleId.Value != Guid.Empty && 
            domainEntity.Vehicle != null && !string.IsNullOrEmpty(domainEntity.Vehicle.LicensePlate))
        {
            // Use the LicensePlate from the loaded Vehicle entity
            idVehiculo = domainEntity.Vehicle.LicensePlate;
        }
        else
        {
            // Use placeholder when VehicleId is null or Vehicle is not loaded
            idVehiculo = "SIN-VEHICULO";
        }

        return new DbRoute
        {
            // IDs - Conversión de Guid a long
            IdRuta = !string.IsNullOrEmpty(domainEntity.Id) && Guid.TryParse(domainEntity.Id, out var routeGuid)
                ? GuidLongConverter.ToLong(routeGuid) 
                : 0,
            
            // Basic Info
            Nombre = domainEntity.Name,
            
            // Assignment
            IdVehiculo = idVehiculo, // Mandatory in BD
            IdResponsable = GuidStringConverter.ToString(domainEntity.DriverId ?? Guid.Empty),
            
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
        if (domainEntity.VehicleId.HasValue && domainEntity.VehicleId.Value != Guid.Empty &&
            domainEntity.Vehicle != null && !string.IsNullOrEmpty(domainEntity.Vehicle.LicensePlate))
        {
            // Use the LicensePlate from the loaded Vehicle entity
            dbEntity.IdVehiculo = domainEntity.Vehicle.LicensePlate;
        }
        else if (domainEntity.VehicleId == null)
        {
            // Keep placeholder if VehicleId is null
            dbEntity.IdVehiculo = "SIN-VEHICULO";
        }
        
        // Update DriverId if provided
        if (domainEntity.DriverId.HasValue)
        {
            dbEntity.IdResponsable = GuidStringConverter.ToString(domainEntity.DriverId.Value);
        }
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

