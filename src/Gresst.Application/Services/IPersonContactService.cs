using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service interface for managing PersonContacts
/// </summary>
public interface IPersonContactService
{
    // Account Person Contacts
    Task<IEnumerable<PersonContactDto>> GetAccountPersonContactsAsync(CancellationToken cancellationToken = default);
    Task<PersonContactDto?> GetAccountPersonContactAsync(Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    Task<PersonContactDto> CreateAccountPersonContactAsync(CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> UpdateAccountPersonContactAsync(UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountPersonContactAsync(Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    
    // Client Contacts
    Task<IEnumerable<PersonContactDto>> GetClientContactsAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> GetClientContactAsync(Guid clientId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    Task<PersonContactDto> CreateClientContactAsync(Guid clientId, CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> UpdateClientContactAsync(Guid clientId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteClientContactAsync(Guid clientId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    
    // Provider Contacts
    Task<IEnumerable<PersonContactDto>> GetProviderContactsAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> GetProviderContactAsync(Guid providerId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    Task<PersonContactDto> CreateProviderContactAsync(Guid providerId, CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> UpdateProviderContactAsync(Guid providerId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProviderContactAsync(Guid providerId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
}

