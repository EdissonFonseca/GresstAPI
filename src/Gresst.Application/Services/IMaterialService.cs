using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IMaterialService
{
    // GetAll ahora filtra automáticamente por usuario actual
    Task<IEnumerable<MaterialDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // GetById verifica que el usuario tenga acceso
    Task<MaterialDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<MaterialDto>> GetByWasteClassAsync(Guid wasteTypeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaterialDto>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    
    // Account Person (persona de la cuenta) - Default operations
    Task<IEnumerable<MaterialDto>> GetAccountPersonMaterialsAsync(CancellationToken cancellationToken = default);
    Task<MaterialDto> CreateAccountPersonMaterialAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default);
    
    // Provider operations
    Task<IEnumerable<MaterialDto>> GetProviderMaterialsAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<MaterialDto> CreateProviderMaterialAsync(Guid providerId, CreateMaterialDto dto, CancellationToken cancellationToken = default);
    
    // Client operations
    Task<IEnumerable<MaterialDto>> GetClientMaterialsAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<MaterialDto> CreateClientMaterialAsync(Guid clientId, CreateMaterialDto dto, CancellationToken cancellationToken = default);
    
    // Facility operations (for account person, provider, or client)
    Task<IEnumerable<MaterialDto>> GetFacilityMaterialsAsync(Guid facilityId, CancellationToken cancellationToken = default);
    Task<MaterialDto> CreateFacilityMaterialAsync(Guid facilityId, CreateMaterialDto dto, CancellationToken cancellationToken = default);
    
    Task<MaterialDto> CreateAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default);
    Task<MaterialDto?> UpdateAsync(UpdateMaterialDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Admin puede ver todos los materiales (sin filtrar por usuario)
    Task<IEnumerable<MaterialDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
}

