using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface ITreatmentService
{
    // Treatment CRUD
    Task<IEnumerable<TreatmentDto>> GetAllTreatmentsAsync(CancellationToken cancellationToken = default);
    Task<TreatmentDto?> GetTreatmentByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TreatmentDto> CreateTreatmentAsync(CreateTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<TreatmentDto?> UpdateTreatmentAsync(UpdateTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTreatmentAsync(string id, CancellationToken cancellationToken = default);
    
    // PersonTreatment - Account Person
    Task<IEnumerable<PersonTreatmentDto>> GetAccountPersonTreatmentsAsync(CancellationToken cancellationToken = default);
    Task<PersonTreatmentDto> CreateAccountPersonTreatmentAsync(CreatePersonTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<PersonTreatmentDto?> UpdateAccountPersonTreatmentAsync(UpdatePersonTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountPersonTreatmentAsync(string treatmentId, CancellationToken cancellationToken = default);
    
    // PersonTreatment - Provider
    Task<IEnumerable<PersonTreatmentDto>> GetProviderTreatmentsAsync(string providerId, CancellationToken cancellationToken = default);
    Task<PersonTreatmentDto> CreateProviderTreatmentAsync(string providerId, CreatePersonTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<PersonTreatmentDto?> UpdateProviderTreatmentAsync(string providerId, UpdatePersonTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProviderTreatmentAsync(string providerId, string treatmentId, CancellationToken cancellationToken = default);
}

