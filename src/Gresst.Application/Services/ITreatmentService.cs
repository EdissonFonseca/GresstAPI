using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

public interface ITreatmentService
{
    // Treatment CRUD
    Task<IEnumerable<TreatmentDto>> GetAllTreatmentsAsync(CancellationToken cancellationToken = default);
    Task<TreatmentDto?> GetTreatmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TreatmentDto> CreateTreatmentAsync(CreateTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<TreatmentDto?> UpdateTreatmentAsync(UpdateTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTreatmentAsync(Guid id, CancellationToken cancellationToken = default);
    
    // PersonTreatment - Account Person
    Task<IEnumerable<PersonTreatmentDto>> GetAccountPersonTreatmentsAsync(CancellationToken cancellationToken = default);
    Task<PersonTreatmentDto> CreateAccountPersonTreatmentAsync(CreatePersonTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<PersonTreatmentDto?> UpdateAccountPersonTreatmentAsync(UpdatePersonTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountPersonTreatmentAsync(Guid treatmentId, CancellationToken cancellationToken = default);
    
    // PersonTreatment - Provider
    Task<IEnumerable<PersonTreatmentDto>> GetProviderTreatmentsAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<PersonTreatmentDto> CreateProviderTreatmentAsync(Guid providerId, CreatePersonTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<PersonTreatmentDto?> UpdateProviderTreatmentAsync(Guid providerId, UpdatePersonTreatmentDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProviderTreatmentAsync(Guid providerId, Guid treatmentId, CancellationToken cancellationToken = default);
}

