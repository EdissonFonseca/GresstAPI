using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IFacilityService
{
    // GetAll ahora filtra automáticamente por usuario actual
    Task<IEnumerable<FacilityDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // GetById verifica que el usuario tenga acceso
    Task<FacilityDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<FacilityDto>> GetByPersonAsync(string personId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FacilityDto>> GetByTypeAsync(string facilityType, CancellationToken cancellationToken = default);
    
    // Account Person (persona de la cuenta) - Default operations
    Task<IEnumerable<FacilityDto>> GetAccountPersonFacilitiesAsync(CancellationToken cancellationToken = default);
    Task<FacilityDto> CreateAccountPersonFacilityAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default);
    
    // Provider operations
    Task<IEnumerable<FacilityDto>> GetProviderFacilitiesAsync(string providerId, CancellationToken cancellationToken = default);
    Task<FacilityDto> CreateProviderFacilityAsync(string providerId, CreateFacilityDto dto, CancellationToken cancellationToken = default);
    
    // Customer operations
    Task<IEnumerable<FacilityDto>> GetCustomerFacilitiesAsync(string customerId, CancellationToken cancellationToken = default);
    Task<FacilityDto> CreateCustomerFacilityAsync(string customerId, CreateFacilityDto dto, CancellationToken cancellationToken = default);
    
    Task<FacilityDto> CreateAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default);
    Task<FacilityDto?> UpdateAsync(UpdateFacilityDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    
    // GetAllByAccount mantiene compatibilidad pero también filtra por usuario
    Task<IEnumerable<FacilityDto>> GetAllByAccountAsync(string accountId, CancellationToken cancellationToken = default);
    
    // Admin puede ver todas las facilities (sin filtrar por usuario)
    Task<IEnumerable<FacilityDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
}
