using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IWasteClassService
{
    // WasteClass CRUD
    Task<IEnumerable<WasteClassDto>> GetAllWasteClassesAsync(CancellationToken cancellationToken = default);
    Task<WasteClassDto?> GetWasteClassByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<WasteClassDto> CreateWasteClassAsync(CreateWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<WasteClassDto?> UpdateWasteClassAsync(UpdateWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteWasteClassAsync(string id, CancellationToken cancellationToken = default);
    
    // PersonWasteClass - Account Person
    Task<IEnumerable<PersonWasteClassDto>> GetAccountPersonWasteClassesAsync(CancellationToken cancellationToken = default);
    Task<PersonWasteClassDto> CreateAccountPersonWasteClassAsync(CreatePersonWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<PersonWasteClassDto?> UpdateAccountPersonWasteClassAsync(UpdatePersonWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountPersonWasteClassAsync(string wasteClassId, CancellationToken cancellationToken = default);
    
    // PersonWasteClass - Provider
    Task<IEnumerable<PersonWasteClassDto>> GetProviderWasteClassesAsync(string providerId, CancellationToken cancellationToken = default);
    Task<PersonWasteClassDto> CreateProviderWasteClassAsync(string providerId, CreatePersonWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<PersonWasteClassDto?> UpdateProviderWasteClassAsync(string providerId, UpdatePersonWasteClassDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProviderWasteClassAsync(string providerId, string wasteClassId, CancellationToken cancellationToken = default);
}

