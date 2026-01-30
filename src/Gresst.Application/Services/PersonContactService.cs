using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing PersonContacts
/// Handles contacts for Account Person, Customers, and Providers
/// </summary>
public class PersonContactService : IPersonContactService
{
    private readonly IPersonContactRepository _personContactRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    // Role codes
    private const string CLIENT_ROLE_CODE = "CL";
    private const string PROVIDER_ROLE_CODE = "PR";

    public PersonContactService(
        IPersonContactRepository personContactRepository,
        IPersonRepository personRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _personContactRepository = personContactRepository;
        _personRepository = personRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    #region Account Person Contacts

    public async Task<IEnumerable<PersonContactDto>> GetAccountPersonContactsAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PersonId))
            return Enumerable.Empty<PersonContactDto>();

        var contacts = await _personContactRepository.GetContactsByPersonIdAsync(account.PersonId, cancellationToken);
        return contacts.Select(MapToDto);
    }

    public async Task<PersonContactDto?> GetAccountPersonContactAsync(string contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PersonId))
            return null;

        var contact = await _personContactRepository.GetByPersonAndContactIdAsync(
            account.PersonId, contactId, relationshipType, cancellationToken);
        
        return contact != null ? MapToDto(contact) : null;
    }

    public async Task<PersonContactDto> CreateAccountPersonContactAsync(CreatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PersonId))
            throw new InvalidOperationException("Account or Account Person not found");

        var personContact = new PersonContact
        {
            Id = string.Empty,
            AccountId = accountId,
            PersonId = account.PersonId,
            ContactId = dto.ContactId,
            RelationshipType = dto.RelationshipType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            RequiresReconciliation = dto.RequiresReconciliation,
            SendEmail = dto.SendEmail,
            Email = dto.Email,
            Phone = dto.Phone,
            Phone2 = dto.Phone2,
            Address = dto.Address,
            Name = dto.Name,
            JobTitle = dto.JobTitle,
            WebPage = dto.WebPage,
            Signature = dto.Signature,
            LocationId = dto.LocationId,
            Notes = dto.Notes,
            AdditionalData = dto.AdditionalData,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _personContactRepository.AddAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _personContactRepository.GetByPersonAndContactIdAsync(
            account.PersonId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        return created != null ? MapToDto(created) : throw new InvalidOperationException("Failed to create contact");
    }

    public async Task<PersonContactDto?> UpdateAccountPersonContactAsync(UpdatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PersonId))
            return null;

        var personContact = await _personContactRepository.GetByPersonAndContactIdAsync(
            account.PersonId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        if (personContact == null)
            return null;

        // Update fields
        if (!string.IsNullOrEmpty(dto.RelationshipType))
            personContact.RelationshipType = dto.RelationshipType;
        if (dto.StartDate.HasValue)
            personContact.StartDate = dto.StartDate;
        if (dto.EndDate.HasValue)
            personContact.EndDate = dto.EndDate;
        if (dto.Status != null)
            personContact.Status = dto.Status;
        if (dto.RequiresReconciliation.HasValue)
            personContact.RequiresReconciliation = dto.RequiresReconciliation;
        if (dto.SendEmail.HasValue)
            personContact.SendEmail = dto.SendEmail;
        if (dto.Email != null)
            personContact.Email = dto.Email;
        if (dto.Phone != null)
            personContact.Phone = dto.Phone;
        if (dto.Phone2 != null)
            personContact.Phone2 = dto.Phone2;
        if (dto.Address != null)
            personContact.Address = dto.Address;
        if (dto.Name != null)
            personContact.Name = dto.Name;
        if (dto.JobTitle != null)
            personContact.JobTitle = dto.JobTitle;
        if (dto.WebPage != null)
            personContact.WebPage = dto.WebPage;
        if (dto.Signature != null)
            personContact.Signature = dto.Signature;
        if (!string.IsNullOrEmpty(dto.LocationId))
            personContact.LocationId = dto.LocationId;
        if (dto.Notes != null)
            personContact.Notes = dto.Notes;
        if (dto.AdditionalData != null)
            personContact.AdditionalData = dto.AdditionalData;
        if (dto.IsActive.HasValue)
            personContact.IsActive = dto.IsActive.Value;

        personContact.UpdatedAt = DateTime.UtcNow;

        await _personContactRepository.UpdateAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _personContactRepository.GetByPersonAndContactIdAsync(
            account.PersonId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        return updated != null ? MapToDto(updated) : null;
    }

    public async Task<bool> DeleteAccountPersonContactAsync(string contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PersonId))
            return false;

        var personContact = await _personContactRepository.GetByPersonAndContactIdAsync(
            account.PersonId, contactId, relationshipType, cancellationToken);
        
        if (personContact == null)
            return false;

        await _personContactRepository.DeleteAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    #endregion

    #region Person Contacts (Generic - works for any person)

    public async Task<IEnumerable<PersonContactDto>> GetPersonContactsAsync(string personId, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (person == null)
            return Enumerable.Empty<PersonContactDto>();

        var contacts = await _personContactRepository.GetContactsByPersonIdAsync(personId, cancellationToken);
        return contacts.Select(MapToDto);
    }

    public async Task<PersonContactDto?> GetPersonContactAsync(string personId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (person == null)
            return null;

        var contact = await _personContactRepository.GetByPersonAndContactIdAsync(
            personId, contactId, relationshipType, cancellationToken);
        
        return contact != null ? MapToDto(contact) : null;
    }

    public async Task<PersonContactDto> CreatePersonContactAsync(string personId, CreatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (person == null)
            throw new InvalidOperationException("Person not found");

        var accountId = _currentUserService.GetCurrentAccountId();
        var personContact = new PersonContact
        {
            Id = string.Empty,
            AccountId = accountId,
            PersonId = personId,
            ContactId = dto.ContactId,
            RelationshipType = dto.RelationshipType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            RequiresReconciliation = dto.RequiresReconciliation,
            SendEmail = dto.SendEmail,
            Email = dto.Email,
            Phone = dto.Phone,
            Phone2 = dto.Phone2,
            Address = dto.Address,
            Name = dto.Name,
            JobTitle = dto.JobTitle,
            WebPage = dto.WebPage,
            Signature = dto.Signature,
            LocationId = dto.LocationId,
            Notes = dto.Notes,
            AdditionalData = dto.AdditionalData,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _personContactRepository.AddAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _personContactRepository.GetByPersonAndContactIdAsync(
            personId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        return created != null ? MapToDto(created) : throw new InvalidOperationException("Failed to create contact");
    }

    public async Task<PersonContactDto?> UpdatePersonContactAsync(string personId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (person == null)
            return null;

        var personContact = await _personContactRepository.GetByPersonAndContactIdAsync(
            personId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        if (personContact == null)
            return null;

        // Update fields
        UpdatePersonContactFields(personContact, dto);

        await _personContactRepository.UpdateAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _personContactRepository.GetByPersonAndContactIdAsync(
            personId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        return updated != null ? MapToDto(updated) : null;
    }

    public async Task<bool> DeletePersonContactAsync(string personId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAsync(personId, cancellationToken);
        if (person == null)
            return false;

        var personContact = await _personContactRepository.GetByPersonAndContactIdAsync(
            personId, contactId, relationshipType, cancellationToken);
        
        if (personContact == null)
            return false;

        await _personContactRepository.DeleteAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    #endregion

    #region Customer Contacts (Legacy - delegate to generic methods)

    public async Task<IEnumerable<PersonContactDto>> GetCustomerContactsAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await GetPersonContactsAsync(customerId, cancellationToken);
    }

    public async Task<PersonContactDto?> GetCustomerContactAsync(string customerId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        return await GetPersonContactAsync(customerId, contactId, relationshipType, cancellationToken);
    }

    public async Task<PersonContactDto> CreateCustomerContactAsync(string customerId, CreatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        return await CreatePersonContactAsync(customerId, dto, cancellationToken);
    }

    public async Task<PersonContactDto?> UpdateCustomerContactAsync(string customerId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        return await UpdatePersonContactAsync(customerId, dto, cancellationToken);
    }

    public async Task<bool> DeleteCustomerContactAsync(string customerId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        return await DeletePersonContactAsync(customerId, contactId, relationshipType, cancellationToken);
    }

    #endregion

    #region Provider Contacts (Legacy - delegate to generic methods)

    public async Task<IEnumerable<PersonContactDto>> GetProviderContactsAsync(string providerId, CancellationToken cancellationToken = default)
    {
        return await GetPersonContactsAsync(providerId, cancellationToken);
    }

    public async Task<PersonContactDto?> GetProviderContactAsync(string providerId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        return await GetPersonContactAsync(providerId, contactId, relationshipType, cancellationToken);
    }

    public async Task<PersonContactDto> CreateProviderContactAsync(string providerId, CreatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        return await CreatePersonContactAsync(providerId, dto, cancellationToken);
    }

    public async Task<PersonContactDto?> UpdateProviderContactAsync(string providerId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        return await UpdatePersonContactAsync(providerId, dto, cancellationToken);
    }

    public async Task<bool> DeleteProviderContactAsync(string providerId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        return await DeletePersonContactAsync(providerId, contactId, relationshipType, cancellationToken);
    }


    #endregion

    #region Helper Methods

    private void UpdatePersonContactFields(PersonContact personContact, UpdatePersonContactDto dto)
    {
        if (!string.IsNullOrEmpty(dto.RelationshipType))
            personContact.RelationshipType = dto.RelationshipType;
        if (dto.StartDate.HasValue)
            personContact.StartDate = dto.StartDate;
        if (dto.EndDate.HasValue)
            personContact.EndDate = dto.EndDate;
        if (dto.Status != null)
            personContact.Status = dto.Status;
        if (dto.RequiresReconciliation.HasValue)
            personContact.RequiresReconciliation = dto.RequiresReconciliation;
        if (dto.SendEmail.HasValue)
            personContact.SendEmail = dto.SendEmail;
        if (dto.Email != null)
            personContact.Email = dto.Email;
        if (dto.Phone != null)
            personContact.Phone = dto.Phone;
        if (dto.Phone2 != null)
            personContact.Phone2 = dto.Phone2;
        if (dto.Address != null)
            personContact.Address = dto.Address;
        if (dto.Name != null)
            personContact.Name = dto.Name;
        if (dto.JobTitle != null)
            personContact.JobTitle = dto.JobTitle;
        if (dto.WebPage != null)
            personContact.WebPage = dto.WebPage;
        if (dto.Signature != null)
            personContact.Signature = dto.Signature;
        if (!string.IsNullOrEmpty(dto.LocationId))
            personContact.LocationId = dto.LocationId;
        if (dto.Notes != null)
            personContact.Notes = dto.Notes;
        if (dto.AdditionalData != null)
            personContact.AdditionalData = dto.AdditionalData;
        if (dto.IsActive.HasValue)
            personContact.IsActive = dto.IsActive.Value;

        personContact.UpdatedAt = DateTime.UtcNow;
    }

    private PersonContactDto MapToDto(PersonContact personContact)
    {
        return new PersonContactDto
        {
            Id = personContact.Id,
            AccountId = personContact.AccountId,
            PersonId = personContact.PersonId,
            PersonName = personContact.Person?.Name,
            ContactId = personContact.ContactId,
            ContactName = personContact.Contact?.Name,
            ContactDocumentNumber = personContact.Contact?.DocumentNumber,
            RelationshipType = personContact.RelationshipType,
            StartDate = personContact.StartDate,
            EndDate = personContact.EndDate,
            Status = personContact.Status,
            RequiresReconciliation = personContact.RequiresReconciliation,
            SendEmail = personContact.SendEmail,
            Email = personContact.Email,
            Phone = personContact.Phone,
            Phone2 = personContact.Phone2,
            Address = personContact.Address,
            Name = personContact.Name,
            JobTitle = personContact.JobTitle,
            WebPage = personContact.WebPage,
            Signature = personContact.Signature,
            LocationId = personContact.LocationId,
            LocationName = personContact.Location?.Name,
            Notes = personContact.Notes,
            AdditionalData = personContact.AdditionalData,
            CreatedAt = personContact.CreatedAt,
            UpdatedAt = personContact.UpdatedAt,
            IsActive = personContact.IsActive
        };
    }

    #endregion
}

