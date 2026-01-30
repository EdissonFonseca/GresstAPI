using Gresst.Application.DTOs;

namespace Gresst.Application.Services;

/// <summary>
/// Service interface for managing PersonContacts
/// </summary>
public interface IPersonContactService
{
    // Account Person Contacts
    Task<IEnumerable<PersonContactDto>> GetAccountPersonContactsAsync(CancellationToken cancellationToken = default);
    Task<PersonContactDto?> GetAccountPersonContactAsync(string contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    Task<PersonContactDto> CreateAccountPersonContactAsync(CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> UpdateAccountPersonContactAsync(UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountPersonContactAsync(string contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    
    // Person Contacts (generic - works for any person, client or provider)
    Task<IEnumerable<PersonContactDto>> GetPersonContactsAsync(string personId, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> GetPersonContactAsync(string personId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    Task<PersonContactDto> CreatePersonContactAsync(string personId, CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<PersonContactDto?> UpdatePersonContactAsync(string personId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeletePersonContactAsync(string personId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    
    // Legacy methods (for backward compatibility - delegate to generic methods)
    [Obsolete("Use GetPersonContactsAsync instead")]
    Task<IEnumerable<PersonContactDto>> GetCustomerContactsAsync(string customerId, CancellationToken cancellationToken = default);
    [Obsolete("Use GetPersonContactAsync instead")]
    Task<PersonContactDto?> GetCustomerContactAsync(string customerId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    [Obsolete("Use CreatePersonContactAsync instead")]
    Task<PersonContactDto> CreateCustomerContactAsync(string customerId, CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    [Obsolete("Use UpdatePersonContactAsync instead")]
    Task<PersonContactDto?> UpdateCustomerContactAsync(string customerId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    [Obsolete("Use DeletePersonContactAsync instead")]
    Task<bool> DeleteCustomerContactAsync(string customerId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    
    [Obsolete("Use GetPersonContactsAsync instead")]
    Task<IEnumerable<PersonContactDto>> GetProviderContactsAsync(string providerId, CancellationToken cancellationToken = default);
    [Obsolete("Use GetPersonContactAsync instead")]
    Task<PersonContactDto?> GetProviderContactAsync(string providerId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
    [Obsolete("Use CreatePersonContactAsync instead")]
    Task<PersonContactDto> CreateProviderContactAsync(string providerId, CreatePersonContactDto dto, CancellationToken cancellationToken = default);
    [Obsolete("Use UpdatePersonContactAsync instead")]
    Task<PersonContactDto?> UpdateProviderContactAsync(string providerId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default);
    [Obsolete("Use DeletePersonContactAsync instead")]
    Task<bool> DeleteProviderContactAsync(string providerId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default);
}

