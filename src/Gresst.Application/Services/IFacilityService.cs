using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IFacilityService
{
    Task<FacilityDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FacilityDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FacilityDto>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<IEnumerable<FacilityDto>> GetByTypeAsync(string facilityType, CancellationToken cancellationToken = default);
    Task<FacilityDto> CreateAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default);
    Task<FacilityDto> UpdateAsync(UpdateFacilityDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

