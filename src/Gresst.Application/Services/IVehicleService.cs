using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IVehicleService
{
    // GetAll ahora filtra automáticamente por usuario actual (con segmentación)
    Task<IEnumerable<VehicleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // GetById verifica que el usuario tenga acceso
    Task<VehicleDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<VehicleDto>> GetByPersonAsync(string personId, CancellationToken cancellationToken = default);
    
    // Account Person (persona de la cuenta) - Default operations
    Task<IEnumerable<VehicleDto>> GetAccountPersonVehiclesAsync(CancellationToken cancellationToken = default);
    Task<VehicleDto> CreateAccountPersonVehicleAsync(CreateVehicleDto dto, CancellationToken cancellationToken = default);
    
    // Provider operations
    Task<IEnumerable<VehicleDto>> GetProviderVehiclesAsync(string providerId, CancellationToken cancellationToken = default);
    Task<VehicleDto> CreateProviderVehicleAsync(string providerId, CreateVehicleDto dto, CancellationToken cancellationToken = default);
    
    // Customer operations
    Task<IEnumerable<VehicleDto>> GetCustomerVehiclesAsync(string customerId, CancellationToken cancellationToken = default);
    Task<VehicleDto> CreateCustomerVehicleAsync(string customerId, CreateVehicleDto dto, CancellationToken cancellationToken = default);
    
    Task<VehicleDto> CreateAsync(CreateVehicleDto dto, CancellationToken cancellationToken = default);
    Task<VehicleDto?> UpdateAsync(UpdateVehicleDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    
    // Admin puede ver todos los vehículos (sin filtrar por usuario)
    Task<IEnumerable<VehicleDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
}

