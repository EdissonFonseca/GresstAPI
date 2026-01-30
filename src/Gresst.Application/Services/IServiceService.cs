using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface IServiceService
{
    // Service CRUD
    Task<IEnumerable<ServiceDto>> GetAllServicesAsync(CancellationToken cancellationToken = default);
    Task<ServiceDto?> GetServiceByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto, CancellationToken cancellationToken = default);
    Task<ServiceDto?> UpdateServiceAsync(UpdateServiceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteServiceAsync(string id, CancellationToken cancellationToken = default);
    
    // PersonService - Account Person
    Task<IEnumerable<PersonServiceDto>> GetAccountPersonServicesAsync(CancellationToken cancellationToken = default);
    Task<PersonServiceDto> CreateAccountPersonServiceAsync(CreatePersonServiceDto dto, CancellationToken cancellationToken = default);
    Task<PersonServiceDto?> UpdateAccountPersonServiceAsync(UpdatePersonServiceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountPersonServiceAsync(string serviceId, DateTime startDate, CancellationToken cancellationToken = default);
    
    // PersonService - Provider
    Task<IEnumerable<PersonServiceDto>> GetProviderServicesAsync(string providerId, CancellationToken cancellationToken = default);
    Task<PersonServiceDto> CreateProviderServiceAsync(string providerId, CreatePersonServiceDto dto, CancellationToken cancellationToken = default);
    Task<PersonServiceDto?> UpdateProviderServiceAsync(string providerId, UpdatePersonServiceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProviderServiceAsync(string providerId, string serviceId, DateTime startDate, CancellationToken cancellationToken = default);
}

