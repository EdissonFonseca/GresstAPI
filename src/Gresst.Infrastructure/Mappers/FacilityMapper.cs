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
            Id = dbEntity.IdDeposito.ToString(),
            Name = dbEntity.Nombre ?? string.Empty,
            Address = dbEntity.Direccion,
            Phone = dbEntity.Telefono,
            Email = dbEntity.Correo,
            LocalityId = dbEntity.IdLocalizacion?.ToString(),  
            IsActive = dbEntity.Activo,
            ParentId = dbEntity.IdUbicacion?.ToString(),
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
            IdDeposito = long.TryParse(domainEntity.Id, out var idDeposito) ? idDeposito : 0,
            IdCuenta = long.TryParse(accountId, out var idCuenta) ? idCuenta : 0,
            Nombre = domainEntity.Name,
            Direccion = domainEntity.Address,
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
        dbEntity.Direccion = domainEntity.Address;
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