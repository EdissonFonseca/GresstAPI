using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IPackagingService
{
    Task<IEnumerable<PackagingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PackagingDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PackagingDto>> GetByTypeAsync(string packagingType, CancellationToken cancellationToken = default);
    Task<PackagingDto> CreateAsync(CreatePackagingDto dto, CancellationToken cancellationToken = default);
    Task<PackagingDto?> UpdateAsync(UpdatePackagingDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

