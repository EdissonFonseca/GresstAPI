namespace Gresst.Application.Services.Interfaces;

/// <summary>
/// Service to check if current user has access to specific resources
/// Implements row-level security based on UsuarioDeposito, UsuarioVehiculo, etc.
/// </summary>
public interface IDataSegmentationService
{
    // Facilities - facility Id is string (BaseEntity.Id)
    Task<bool> UserHasAccessToFacilityAsync(string facilityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserFacilityIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignFacilityToUserAsync(string userId, string facilityId, CancellationToken cancellationToken = default);
    Task<bool> RevokeFacilityFromUserAsync(string userId, string facilityId, CancellationToken cancellationToken = default);
    
    // Vehicles - vehicle Id is string (BaseEntity.Id / IdVehiculo in BD)
    Task<bool> UserHasAccessToVehicleAsync(string vehicleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserVehicleIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignVehicleToUserAsync(string userId, string vehicleId, CancellationToken cancellationToken = default);
    Task<bool> RevokeVehicleFromUserAsync(string userId, string vehicleId, CancellationToken cancellationToken = default);
    
    // Materials - material Id is string (BaseEntity.Id, BD long as string)
    Task<bool> UserHasAccessToMaterialAsync(string materialId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserMaterialIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignMaterialToUserAsync(string userId, string materialId, CancellationToken cancellationToken = default);
    Task<bool> RevokeMaterialFromUserAsync(string userId, string materialId, CancellationToken cancellationToken = default);
    
    // Generic check
    Task<bool> CurrentUserIsAdminAsync(CancellationToken cancellationToken = default);
}

