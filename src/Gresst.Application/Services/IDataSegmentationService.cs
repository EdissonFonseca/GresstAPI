namespace Gresst.Application.Services;

/// <summary>
/// Service to check if current user has access to specific resources
/// Implements row-level security based on UsuarioDeposito, UsuarioVehiculo, etc.
/// </summary>
public interface IDataSegmentationService
{
    // Facilities - facility Id is string (BaseEntity.Id)
    Task<bool> UserHasAccessToFacilityAsync(string facilityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserFacilityIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignFacilityToUserAsync(Guid userId, Guid facilityId, CancellationToken cancellationToken = default);
    Task<bool> RevokeFacilityFromUserAsync(Guid userId, Guid facilityId, CancellationToken cancellationToken = default);
    
    // Vehicles - vehicle Id is string (BaseEntity.Id / IdVehiculo in BD)
    Task<bool> UserHasAccessToVehicleAsync(string vehicleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserVehicleIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignVehicleToUserAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default);
    Task<bool> RevokeVehicleFromUserAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default);
    
    // Materials - material Id is string (BaseEntity.Id, BD long as string)
    Task<bool> UserHasAccessToMaterialAsync(string materialId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserMaterialIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignMaterialToUserAsync(Guid userId, Guid materialId, CancellationToken cancellationToken = default);
    Task<bool> RevokeMaterialFromUserAsync(Guid userId, Guid materialId, CancellationToken cancellationToken = default);
    
    // Generic check
    Task<bool> CurrentUserIsAdminAsync(CancellationToken cancellationToken = default);
}

