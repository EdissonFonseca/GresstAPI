using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IMaterialService
{
    // GetAll ahora filtra automáticamente por usuario actual
    Task<IEnumerable<MaterialDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // GetById verifica que el usuario tenga acceso
    Task<MaterialDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<MaterialDto>> GetByWasteClassAsync(string wasteTypeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaterialDto>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    
    // Account Person (persona de la cuenta) - Default operations
    Task<IEnumerable<MaterialDto>> GetAccountPersonMaterialsAsync(CancellationToken cancellationToken = default);
    Task<MaterialDto> CreateAccountPersonMaterialAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default);
    
    // Provider operations
    Task<IEnumerable<MaterialDto>> GetProviderMaterialsAsync(string providerId, CancellationToken cancellationToken = default);
    Task<MaterialDto> CreateProviderMaterialAsync(string providerId, CreateMaterialDto dto, CancellationToken cancellationToken = default);
    
    // Customer operations
    Task<IEnumerable<MaterialDto>> GetCustomerMaterialsAsync(string customerId, CancellationToken cancellationToken = default);
    Task<MaterialDto> CreateCustomerMaterialAsync(string customerId, CreateMaterialDto dto, CancellationToken cancellationToken = default);
    
    // Facility operations (for account person, provider, or client)
    Task<IEnumerable<MaterialDto>> GetFacilityMaterialsAsync(string facilityId, CancellationToken cancellationToken = default);
    Task<MaterialDto> CreateFacilityMaterialAsync(string facilityId, CreateMaterialDto dto, CancellationToken cancellationToken = default);
    
    Task<MaterialDto> CreateAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default);
    Task<MaterialDto?> UpdateAsync(UpdateMaterialDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    
    // Admin puede ver todos los materiales (sin filtrar por usuario)
    Task<IEnumerable<MaterialDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
}

