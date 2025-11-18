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
    
    // Person Contacts (generic - works for any person, client or provider)
    Task<IEnumerable<PersonContactDto>> GetPersonContactsAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> GetPersonContactAsync(Guid personId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    Task<PersonContactDto> CreatePersonContactAsync(Guid personId, CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> UpdatePersonContactAsync(Guid personId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeletePersonContactAsync(Guid personId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    
    // Legacy methods (for backward compatibility - delegate to generic methods)
    [Obsolete("Use GetPersonContactsAsync instead")]
    Task<IEnumerable<PersonContactDto>> GetClientContactsAsync(Guid clientId, CancellationToken cancellationToken = default);
    [Obsolete("Use GetPersonContactAsync instead")]
    Task<PersonContactDto?> GetClientContactAsync(Guid clientId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    [Obsolete("Use CreatePersonContactAsync instead")]
    Task<PersonContactDto> CreateClientContactAsync(Guid clientId, CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    [Obsolete("Use UpdatePersonContactAsync instead")]
    Task<PersonContactDto?> UpdateClientContactAsync(Guid clientId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    [Obsolete("Use DeletePersonContactAsync instead")]
    Task<bool> DeleteClientContactAsync(Guid clientId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    
    [Obsolete("Use GetPersonContactsAsync instead")]
    Task<IEnumerable<PersonContactDto>> GetProviderContactsAsync(Guid providerId, CancellationToken cancellationToken = default);
    [Obsolete("Use GetPersonContactAsync instead")]
    Task<PersonContactDto?> GetProviderContactAsync(Guid providerId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    [Obsolete("Use CreatePersonContactAsync instead")]
    Task<PersonContactDto> CreateProviderContactAsync(Guid providerId, CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    [Obsolete("Use UpdatePersonContactAsync instead")]
    Task<PersonContactDto?> UpdateProviderContactAsync(Guid providerId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    [Obsolete("Use DeletePersonContactAsync instead")]
    Task<bool> DeleteProviderContactAsync(Guid providerId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
}

