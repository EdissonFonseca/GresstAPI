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
    
    Task<FacilityDto> CreateAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default);
    Task<FacilityDto?> UpdateAsync(UpdateFacilityDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // GetAllByAccount mantiene compatibilidad pero también filtra por usuario
    Task<IEnumerable<FacilityDto>> GetAllByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    // Admin puede ver todas las facilities (sin filtrar por usuario)
    Task<IEnumerable<FacilityDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
}
