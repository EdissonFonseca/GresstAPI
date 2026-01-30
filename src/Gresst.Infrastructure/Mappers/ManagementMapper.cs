using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data.Entities;

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// Mapper: Management (Domain/Inglés) ↔ Gestion (BD/Español)
/// </summary>
public class ManagementMapper : MapperBase<Management, Gestion>
{
    /// <summary>
    /// BD → Domain: Gestion → Management
    /// </summary>
    public override Management ToDomain(Gestion dbEntity)
    {
        if (dbEntity == null) 
            throw new ArgumentNullException(nameof(dbEntity));

        return new Management
        {
            // IDs - Domain BaseEntity uses string
            Id = IdConversion.ToStringFromLong(dbEntity.IdMovimiento),
            AccountId = string.Empty, // Se obtiene del usuario actual en DbContext
            
            // Basic Info
            Code = $"MGT-{dbEntity.IdMovimiento}",
            Type = MapServiceToManagementType(dbEntity.IdServicio),
            ExecutedAt = dbEntity.Fecha,
            
            // Waste
            WasteId = IdConversion.ToStringFromLong(dbEntity.IdResiduo),
            
            // Quantity
            Quantity = dbEntity.Peso ?? dbEntity.Cantidad ?? 0,
            Unit = UnitOfMeasure.Kilogram,
            
            // Executor - DB IdResponsable is string
            ExecutedById = dbEntity.IdResponsable ?? string.Empty,
            
            // Origin and Destination
            OriginFacilityId = dbEntity.IdDepositoOrigen != 0 
                ? IdConversion.ToStringFromLong(dbEntity.IdDepositoOrigen) 
                : null,
            DestinationFacilityId = dbEntity.IdDepositoDestino.HasValue 
                ? IdConversion.ToStringFromLong(dbEntity.IdDepositoDestino.Value) 
                : null,
            
            // Related entities
            OrderId = dbEntity.IdOrden.HasValue 
                ? IdConversion.ToStringFromLong(dbEntity.IdOrden.Value) 
                : null,
            VehicleId = dbEntity.IdVehiculo ?? string.Empty,
            TreatmentId = dbEntity.IdTratamiento.HasValue 
                ? IdConversion.ToStringFromLong(dbEntity.IdTratamiento.Value) 
                : null,
            CertificateId = dbEntity.IdCertificado.HasValue 
                ? IdConversion.ToStringFromLong(dbEntity.IdCertificado.Value) 
                : null,
            
            // Notes
            Notes = dbEntity.Observaciones,
            AttachmentUrls = dbEntity.Soportes,
            
            // Audit
            CreatedAt = dbEntity.FechaCreacion,
            UpdatedAt = dbEntity.FechaUltimaModificacion,
            CreatedBy = dbEntity.IdUsuarioCreacion.ToString(),
            UpdatedBy = dbEntity.IdUsuarioUltimaModificacion?.ToString(),
            IsActive = true
        };
    }

    /// <summary>
    /// Domain → BD: Management → Gestion
    /// </summary>
    public override Gestion ToDatabase(Management domainEntity)
    {
        if (domainEntity == null) 
            throw new ArgumentNullException(nameof(domainEntity));

        return new Gestion
        {
            // IDs
            IdMovimiento = IdConversion.ToLongFromString(domainEntity.Id),
            IdResiduo = IdConversion.ToLongFromString(domainEntity.WasteId),
            
            // Service Type
            IdServicio = MapManagementTypeToService(domainEntity.Type),
            
            // Executor - DB IdResponsable is string
            IdResponsable = string.IsNullOrEmpty(domainEntity.ExecutedById) ? null : domainEntity.ExecutedById,
            
            // Date
            Fecha = domainEntity.ExecutedAt,
            
            // Quantity
            Peso = domainEntity.Quantity,
            Cantidad = domainEntity.Quantity,
            Porcentaje = 100,
            
            // Origin and Destination
            IdDepositoOrigen = !string.IsNullOrEmpty(domainEntity.OriginFacilityId) 
                ? IdConversion.ToLongFromString(domainEntity.OriginFacilityId) 
                : 0,
            IdDepositoDestino = !string.IsNullOrEmpty(domainEntity.DestinationFacilityId) 
                ? IdConversion.ToLongFromString(domainEntity.DestinationFacilityId) 
                : null,
            IdPlanta = !string.IsNullOrEmpty(domainEntity.DestinationFacilityId) 
                ? IdConversion.ToLongFromString(domainEntity.DestinationFacilityId) 
                : null,
            
            // Related
            IdOrden = !string.IsNullOrEmpty(domainEntity.OrderId) 
                ? IdConversion.ToLongFromString(domainEntity.OrderId) 
                : null,
            IdVehiculo = string.IsNullOrEmpty(domainEntity.VehicleId) ? null : domainEntity.VehicleId,
            IdTratamiento = !string.IsNullOrEmpty(domainEntity.TreatmentId) 
                ? IdConversion.ToLongFromString(domainEntity.TreatmentId) 
                : null,
            IdCertificado = !string.IsNullOrEmpty(domainEntity.CertificateId) 
                ? IdConversion.ToLongFromString(domainEntity.CertificateId) 
                : null,
            
            // Notes
            Observaciones = domainEntity.Notes,
            Soportes = domainEntity.AttachmentUrls,
            
            // Status
            Procesado = false,
            
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

    public override void UpdateDatabase(Management domainEntity, Gestion dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;

        // Update modifiable fields
        dbEntity.Observaciones = domainEntity.Notes;
        dbEntity.Procesado = true;
        dbEntity.FechaUltimaModificacion = domainEntity.UpdatedAt;
        dbEntity.IdUsuarioUltimaModificacion = !string.IsNullOrEmpty(domainEntity.UpdatedBy) 
            ? long.Parse(domainEntity.UpdatedBy) 
            : null;
    }

    // Service Type mapping
    private ManagementType MapServiceToManagementType(long idServicio)
    {
        // Mapeo basado en IDs comunes de servicios
        // Ajustar según tu BD real
        return idServicio switch
        {
            1 => ManagementType.Generate,
            2 => ManagementType.Collect,
            3 => ManagementType.Transport,
            4 => ManagementType.Receive,
            5 => ManagementType.Store,
            6 => ManagementType.Dispose,
            7 => ManagementType.Treat,
            8 => ManagementType.Transform,
            9 => ManagementType.Deliver,
            10 => ManagementType.Sell,
            11 => ManagementType.Classify,
            _ => ManagementType.Generate
        };
    }

    private long MapManagementTypeToService(ManagementType type)
    {
        return type switch
        {
            ManagementType.Generate => 1,
            ManagementType.Collect => 2,
            ManagementType.Transport => 3,
            ManagementType.Receive => 4,
            ManagementType.Store => 5,
            ManagementType.StoreTemporary => 5,
            ManagementType.StorePermanent => 5,
            ManagementType.Dispose => 6,
            ManagementType.Treat => 7,
            ManagementType.Transform => 8,
            ManagementType.Deliver => 9,
            ManagementType.Sell => 10,
            ManagementType.Classify => 11,
            ManagementType.Transfer => 3,
            _ => 1
        };
    }

}

