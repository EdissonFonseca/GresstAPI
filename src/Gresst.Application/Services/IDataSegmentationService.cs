namespace Gresst.Application.Services;

/// <summary>
/// Service to check if current user has access to specific resources
/// Implements row-level security based on UsuarioDeposito, UsuarioVehiculo, etc.
/// </summary>
public interface IDataSegmentationService
{
    // Facilities
    Task<bool> UserHasAccessToFacilityAsync(Guid facilityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> GetUserFacilityIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignFacilityToUserAsync(Guid userId, Guid facilityId, CancellationToken cancellationToken = default);
    Task<bool> RevokeFacilityFromUserAsync(Guid userId, Guid facilityId, CancellationToken cancellationToken = default);
    
    // Vehicles
    Task<bool> UserHasAccessToVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> GetUserVehicleIdsAsync(CancellationToken cancellationToken = default);
    Task<bool> AssignVehicleToUserAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default);
    Task<bool> RevokeVehicleFromUserAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default);
    
    // Generic check
    Task<bool> CurrentUserIsAdminAsync(CancellationToken cancellationToken = default);
}

