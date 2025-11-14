using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IFacilityService
{
    // GetAll ahora filtra automáticamente por usuario actual
    Task<IEnumerable<FacilityDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // GetById verifica que el usuario tenga acceso
    Task<FacilityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<FacilityDto>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FacilityDto>> GetByTypeAsync(string facilityType, CancellationToken cancellationToken = default);
    
    // Account Person (persona de la cuenta) - Default operations
    Task<IEnumerable<FacilityDto>> GetAccountPersonFacilitiesAsync(CancellationToken cancellationToken = default);
    Task<FacilityDto> CreateAccountPersonFacilityAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default);
    
    // Provider operations
    Task<IEnumerable<FacilityDto>> GetProviderFacilitiesAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<FacilityDto> CreateProviderFacilityAsync(Guid providerId, CreateFacilityDto dto, CancellationToken cancellationToken = default);
    
    // Client operations
    Task<IEnumerable<FacilityDto>> GetClientFacilitiesAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<FacilityDto> CreateClientFacilityAsync(Guid clientId, CreateFacilityDto dto, CancellationToken cancellationToken = default);
    
    Task<FacilityDto> CreateAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default);
    Task<FacilityDto?> UpdateAsync(UpdateFacilityDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // GetAllByAccount mantiene compatibilidad pero también filtra por usuario
    Task<IEnumerable<FacilityDto>> GetAllByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    // Admin puede ver todas las facilities (sin filtrar por usuario)
    Task<IEnumerable<FacilityDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
}
