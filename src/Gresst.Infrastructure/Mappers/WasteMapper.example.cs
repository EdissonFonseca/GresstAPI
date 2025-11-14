using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
// using Gresst.Infrastructure.Data.Entities; // Descomentar después del scaffolding

namespace Gresst.Infrastructure.Mappers;

/// <summary>
/// EJEMPLO de Mapper para Waste ↔ Residuo
/// Este archivo se renombrará a WasteMapper.cs después del scaffolding
/// </summary>
public class WasteMapperExample // : MapperBase<Waste, Residuo>
{
    // EJEMPLO de implementación - ajustar según la BD real
    
    /*
    public override Waste ToDomain(Residuo dbEntity)
    {
        if (dbEntity == null) return null;
        
        return new Waste
        {
            Id = dbEntity.Id,
            AccountId = dbEntity.IdCuenta, // Mapeo de nombres
            Code = dbEntity.Codigo,
            Description = dbEntity.Descripcion,
            WasteClassId = dbEntity.IdTipoResiduo,
            Quantity = dbEntity.Cantidad,
            Unit = (UnitOfMeasure)dbEntity.IdUnidad, // Conversión de enum
            Status = MapStatus(dbEntity.Estado),
            GeneratorId = dbEntity.IdGenerador,
            GeneratedAt = dbEntity.FechaGeneracion,
            CurrentOwnerId = dbEntity.IdPropietarioActual,
            CurrentLocationId = dbEntity.IdUbicacionActual,
            CurrentFacilityId = dbEntity.IdDepositoActual,
            IsHazardous = dbEntity.EsPeligroso,
            IsAvailableInBank = dbEntity.DisponibleBanco ?? false,
            BankPrice = dbEntity.PrecioBanco,
            BatchNumber = dbEntity.NumeroLote,
            CreatedAt = dbEntity.FechaCreacion,
            CreatedBy = dbEntity.CreadoPor,
            UpdatedAt = dbEntity.FechaModificacion,
            UpdatedBy = dbEntity.ModificadoPor,
            IsActive = dbEntity.Activo
        };
    }
    
    public override Residuo ToDatabase(Waste domainEntity)
    {
        if (domainEntity == null) return null;
        
        return new Residuo
        {
            Id = domainEntity.Id,
            IdCuenta = domainEntity.AccountId,
            Codigo = domainEntity.Code,
            Descripcion = domainEntity.Description,
            IdTipoResiduo = domainEntity.WasteClassId,
            Cantidad = domainEntity.Quantity,
            IdUnidad = (int)domainEntity.Unit,
            Estado = MapStatusToDb(domainEntity.Status),
            IdGenerador = domainEntity.GeneratorId,
            FechaGeneracion = domainEntity.GeneratedAt,
            IdPropietarioActual = domainEntity.CurrentOwnerId,
            IdUbicacionActual = domainEntity.CurrentLocationId,
            IdDepositoActual = domainEntity.CurrentFacilityId,
            EsPeligroso = domainEntity.IsHazardous,
            DisponibleBanco = domainEntity.IsAvailableInBank,
            PrecioBanco = domainEntity.BankPrice,
            NumeroLote = domainEntity.BatchNumber,
            FechaCreacion = domainEntity.CreatedAt,
            CreadoPor = domainEntity.CreatedBy,
            FechaModificacion = domainEntity.UpdatedAt,
            ModificadoPor = domainEntity.UpdatedBy,
            Activo = domainEntity.IsActive
        };
    }
    
    public override void UpdateDatabase(Waste domainEntity, Residuo dbEntity)
    {
        if (domainEntity == null || dbEntity == null) return;
        
        // Solo actualizar campos modificables (no IDs, no fechas de creación)
        dbEntity.Descripcion = domainEntity.Description;
        dbEntity.Cantidad = domainEntity.Quantity;
        dbEntity.Estado = MapStatusToDb(domainEntity.Status);
        dbEntity.IdUbicacionActual = domainEntity.CurrentLocationId;
        dbEntity.IdDepositoActual = domainEntity.CurrentFacilityId;
        dbEntity.DisponibleBanco = domainEntity.IsAvailableInBank;
        dbEntity.PrecioBanco = domainEntity.BankPrice;
        dbEntity.FechaModificacion = domainEntity.UpdatedAt;
        dbEntity.ModificadoPor = domainEntity.UpdatedBy;
    }
    
    // Helpers para mapear enums
    private WasteStatus MapStatus(string dbStatus)
    {
        return dbStatus?.ToLower() switch
        {
            "generado" => WasteStatus.Generated,
            "en_transito" => WasteStatus.InTransit,
            "almacenado" => WasteStatus.Stored,
            "en_tratamiento" => WasteStatus.InTreatment,
            "dispuesto" => WasteStatus.Disposed,
            "transformado" => WasteStatus.Transformed,
            "entregado" => WasteStatus.Delivered,
            "vendido" => WasteStatus.Sold,
            _ => WasteStatus.Generated
        };
    }
    
    private string MapStatusToDb(WasteStatus status)
    {
        return status switch
        {
            WasteStatus.Generated => "generado",
            WasteStatus.InTransit => "en_transito",
            WasteStatus.Stored => "almacenado",
            WasteStatus.InTreatment => "en_tratamiento",
            WasteStatus.Disposed => "dispuesto",
            WasteStatus.Transformed => "transformado",
            WasteStatus.Delivered => "entregado",
            WasteStatus.Sold => "vendido",
            WasteStatus.Reused => "reutilizado",
            _ => "generado"
        };
    }
    */
}

