using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IWasteClassService
{
    // WasteClass CRUD
    Task<IEnumerable<WasteClassDto>> GetAllWasteClassesAsync(CancellationToken cancellationToken = default);
    Task<WasteClassDto?> GetWasteClassByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WasteClassDto> CreateWasteClassAsync(CreateWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<WasteClassDto?> UpdateWasteClassAsync(UpdateWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteWasteClassAsync(Guid id, CancellationToken cancellationToken = default);
    
    // PersonWasteClass - Account Person
    Task<IEnumerable<PersonWasteClassDto>> GetAccountPersonWasteClassesAsync(CancellationToken cancellationToken = default);
    Task<PersonWasteClassDto> CreateAccountPersonWasteClassAsync(CreatePersonWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<PersonWasteClassDto?> UpdateAccountPersonWasteClassAsync(UpdatePersonWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountPersonWasteClassAsync(Guid wasteClassId, CancellationToken cancellationToken = default);
    
    // PersonWasteClass - Provider
    Task<IEnumerable<PersonWasteClassDto>> GetProviderWasteClassesAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<PersonWasteClassDto> CreateProviderWasteClassAsync(Guid providerId, CreatePersonWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<PersonWasteClassDto?> UpdateProviderWasteClassAsync(Guid providerId, UpdatePersonWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProviderWasteClassAsync(Guid providerId, Guid wasteClassId, CancellationToken cancellationToken = default);
}

