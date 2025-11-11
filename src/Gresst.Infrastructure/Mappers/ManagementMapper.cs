using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
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
            // IDs
            Id = ConvertLongToGuid(dbEntity.IdMovimiento),
            AccountId = Guid.NewGuid(), // Se obtiene del usuario actual
            
            // Basic Info
            Code = $"MGT-{dbEntity.IdMovimiento}",
            Type = MapServiceToManagementType(dbEntity.IdServicio),
            ExecutedAt = dbEntity.Fecha,
            
            // Waste
            WasteId = ConvertLongToGuid(dbEntity.IdResiduo),
            
            // Quantity
            Quantity = dbEntity.Peso ?? dbEntity.Cantidad ?? 0,
            Unit = UnitOfMeasure.Kilogram,
            
            // Executor
            ExecutedById = !string.IsNullOrEmpty(dbEntity.IdResponsable)
                ? ConvertStringToGuid(dbEntity.IdResponsable)
                : Guid.Empty,
            
            // Origin and Destination
            OriginFacilityId = dbEntity.IdDepositoOrigen != 0 
                ? ConvertLongToGuid(dbEntity.IdDepositoOrigen) 
                : null,
            DestinationFacilityId = dbEntity.IdDepositoDestino.HasValue 
                ? ConvertLongToGuid(dbEntity.IdDepositoDestino.Value) 
                : null,
            
            // Related entities
            OrderId = dbEntity.IdOrden.HasValue 
                ? ConvertLongToGuid(dbEntity.IdOrden.Value) 
                : null,
            VehicleId = !string.IsNullOrEmpty(dbEntity.IdVehiculo)
                ? ConvertStringToGuid(dbEntity.IdVehiculo)
                : null,
            TreatmentId = dbEntity.IdTratamiento.HasValue 
                ? ConvertLongToGuid(dbEntity.IdTratamiento.Value) 
                : null,
            CertificateId = dbEntity.IdCertificado.HasValue 
                ? ConvertLongToGuid(dbEntity.IdCertificado.Value) 
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
            IdMovimiento = ConvertGuidToLong(domainEntity.Id),
            IdResiduo = ConvertGuidToLong(domainEntity.WasteId),
            
            // Service Type
            IdServicio = MapManagementTypeToService(domainEntity.Type),
            
            // Executor
            IdResponsable = domainEntity.ExecutedById != Guid.Empty
                ? ConvertGuidToString(domainEntity.ExecutedById)
                : null,
            
            // Date
            Fecha = domainEntity.ExecutedAt,
            
            // Quantity
            Peso = domainEntity.Quantity,
            Cantidad = domainEntity.Quantity,
            Porcentaje = 100,
            
            // Origin and Destination
            IdDepositoOrigen = domainEntity.OriginFacilityId.HasValue 
                ? ConvertGuidToLong(domainEntity.OriginFacilityId.Value) 
                : 0,
            IdDepositoDestino = domainEntity.DestinationFacilityId.HasValue 
                ? ConvertGuidToLong(domainEntity.DestinationFacilityId.Value) 
                : null,
            IdPlanta = domainEntity.DestinationFacilityId.HasValue 
                ? ConvertGuidToLong(domainEntity.DestinationFacilityId.Value) 
                : null,
            
            // Related
            IdOrden = domainEntity.OrderId.HasValue 
                ? ConvertGuidToLong(domainEntity.OrderId.Value) 
                : null,
            IdVehiculo = domainEntity.VehicleId.HasValue
                ? ConvertGuidToString(domainEntity.VehicleId.Value)
                : null,
            IdTratamiento = domainEntity.TreatmentId.HasValue 
                ? ConvertGuidToLong(domainEntity.TreatmentId.Value) 
                : null,
            IdCertificado = domainEntity.CertificateId.HasValue 
                ? ConvertGuidToLong(domainEntity.CertificateId.Value) 
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

    // Type conversion helpers
    private Guid ConvertLongToGuid(long id)
    {
        if (id == 0) return Guid.Empty;
        return new Guid(id.ToString().PadLeft(32, '0'));
    }

    private Guid ConvertStringToGuid(string id)
    {
        if (string.IsNullOrEmpty(id)) return Guid.Empty;
        
        if (Guid.TryParse(id, out var guid))
            return guid;
        
        return new Guid(id.PadLeft(32, '0').Substring(0, 32));
    }

    private long ConvertGuidToLong(Guid guid)
    {
        if (guid == Guid.Empty) return 0;
        
        var guidString = guid.ToString().Replace("-", "");
        var numericPart = new string(guidString.Where(char.IsDigit).Take(18).ToArray());
        
        return long.TryParse(numericPart, out var result) ? result : 0;
    }

    private string ConvertGuidToString(Guid guid)
    {
        if (guid == Guid.Empty) return string.Empty;
        return guid.ToString().Replace("-", "").Substring(0, 40);
    }
}

