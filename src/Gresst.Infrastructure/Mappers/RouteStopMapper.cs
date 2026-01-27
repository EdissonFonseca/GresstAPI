using Gresst.Domain.Entities;
using Gresst.Infrastructure.Common;
using DomainRouteStop = Gresst.Domain.Entities.RouteStop;
using DbRouteStop = Gresst.Infrastructure.Data.Entities.RutaDeposito;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: RouteStop (Domain/Inglés) ↔ RutaDeposito (BD/Español)
/// Note: RutaDeposito only has RouteId, FacilityId (Deposito), and Sequence (Orden).
/// Other RouteStop properties (LocationId, Address, PersonId, etc.) are not in BD.
/// </summary>
public class RouteStopMapper : MapperBase<DomainRouteStop, DbRouteStop>
{
    /// <summary>
    /// BD → Domain: RutaDeposito → RouteStop
    /// </summary>
    public override DomainRouteStop ToDomain(DbRouteStop dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new DomainRouteStop
        {
            // IDs (composite key - generate a Guid for domain)
            Id = Guid.NewGuid().ToString(),
            AccountId = string.Empty, // RutaDeposito doesn't have AccountId directly
            
            // Route
            RouteId = GuidLongConverter.ToGuid(dbEntity.IdRuta),
            
            // Sequence
            Sequence = dbEntity.Orden,
            
            // Facility (Deposito)
            FacilityId = GuidLongConverter.ToGuid(dbEntity.IdDeposito),
            
            // Other properties not in BD - set to null/default
            LocationId = null,
            Address = null,
            Latitude = null,
            Longitude = null,
            PersonId = null,
            Instructions = null,
            EstimatedTime = null,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = true // RutaDeposito doesn't have Activo field, assume true
        };
    }

    /// <summary>
    /// Domain → BD: RouteStop → RutaDeposito
    /// Note: Only RouteId, FacilityId, and Sequence are saved to BD
    /// </summary>
    public override DbRouteStop ToDatabase(DomainRouteStop domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        if (!domainEntity.FacilityId.HasValue)
            throw new InvalidOperationException("RouteStop must have a FacilityId to be saved to database");

        return new DbRouteStop
        {
            // IDs (composite key)
            IdRuta = GuidLongConverter.ToLong(domainEntity.RouteId),
            IdDeposito = GuidLongConverter.ToLong(domainEntity.FacilityId.Value),
            
            // Sequence
            Orden = domainEntity.Sequence,
            
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

    public override void UpdateDatabase(DomainRouteStop domainEntity, DbRouteStop dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields (IDs are part of composite key, so don't update them)
        dbEntity.Orden = domainEntity.Sequence;
        
        // Update FacilityId if provided (though it's part of composite key, we allow updates)
        if (domainEntity.FacilityId.HasValue)
        {
            dbEntity.IdDeposito = GuidLongConverter.ToLong(domainEntity.FacilityId.Value);
        }
        
        // Audit
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }
}

