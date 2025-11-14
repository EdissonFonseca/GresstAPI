using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IMaterialService
{
    // GetAll ahora filtra automáticamente por usuario actual
    Task<IEnumerable<MaterialDto>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // GetById verifica que el usuario tenga acceso
    Task<MaterialDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<MaterialDto>> GetByWasteTypeAsync(Guid wasteTypeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MaterialDto>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    
    Task<MaterialDto> CreateAsync(CreateMaterialDto dto, CancellationToken cancellationToken = default);
    Task<MaterialDto?> UpdateAsync(UpdateMaterialDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Admin puede ver todos los materiales (sin filtrar por usuario)
    Task<IEnumerable<MaterialDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
}

