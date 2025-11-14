using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing PersonContacts
/// Handles contacts for Account Person, Clients, and Providers
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
        if (account == null || account.PersonId == Guid.Empty)
            return Enumerable.Empty<PersonContactDto>();

        var contacts = await _personContactRepository.GetContactsByPersonIdAsync(account.PersonId, cancellationToken);
        return contacts.Select(MapToDto);
    }

    public async Task<PersonContactDto?> GetAccountPersonContactAsync(Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || account.PersonId == Guid.Empty)
            return null;

        var contact = await _personContactRepository.GetByPersonAndContactIdAsync(
            account.PersonId, contactId, relationshipType, cancellationToken);
        
        return contact != null ? MapToDto(contact) : null;
    }

    public async Task<PersonContactDto> CreateAccountPersonContactAsync(CreatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || account.PersonId == Guid.Empty)
            throw new InvalidOperationException("Account or Account Person not found");

        var personContact = new PersonContact
        {
            Id = Guid.NewGuid(),
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
        if (account == null || account.PersonId == Guid.Empty)
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
        if (dto.LocationId.HasValue)
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

    public async Task<bool> DeleteAccountPersonContactAsync(Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || account.PersonId == Guid.Empty)
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

    #region Client Contacts

    public async Task<IEnumerable<PersonContactDto>> GetClientContactsAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        var client = await _personRepository.GetByIdAndRoleAsync(clientId, CLIENT_ROLE_CODE, cancellationToken);
        if (client == null)
            return Enumerable.Empty<PersonContactDto>();

        var contacts = await _personContactRepository.GetContactsByPersonIdAsync(clientId, cancellationToken);
        return contacts.Select(MapToDto);
    }

    public async Task<PersonContactDto?> GetClientContactAsync(Guid clientId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var client = await _personRepository.GetByIdAndRoleAsync(clientId, CLIENT_ROLE_CODE, cancellationToken);
        if (client == null)
            return null;

        var contact = await _personContactRepository.GetByPersonAndContactIdAsync(
            clientId, contactId, relationshipType, cancellationToken);
        
        return contact != null ? MapToDto(contact) : null;
    }

    public async Task<PersonContactDto> CreateClientContactAsync(Guid clientId, CreatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        var client = await _personRepository.GetByIdAndRoleAsync(clientId, CLIENT_ROLE_CODE, cancellationToken);
        if (client == null)
            throw new InvalidOperationException("Client not found");

        var accountId = _currentUserService.GetCurrentAccountId();
        var personContact = new PersonContact
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            PersonId = clientId,
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
            clientId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        return created != null ? MapToDto(created) : throw new InvalidOperationException("Failed to create contact");
    }

    public async Task<PersonContactDto?> UpdateClientContactAsync(Guid clientId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        var client = await _personRepository.GetByIdAndRoleAsync(clientId, CLIENT_ROLE_CODE, cancellationToken);
        if (client == null)
            return null;

        var personContact = await _personContactRepository.GetByPersonAndContactIdAsync(
            clientId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        if (personContact == null)
            return null;

        // Update fields (same logic as Account Person)
        UpdatePersonContactFields(personContact, dto);

        await _personContactRepository.UpdateAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _personContactRepository.GetByPersonAndContactIdAsync(
            clientId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        return updated != null ? MapToDto(updated) : null;
    }

    public async Task<bool> DeleteClientContactAsync(Guid clientId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var client = await _personRepository.GetByIdAndRoleAsync(clientId, CLIENT_ROLE_CODE, cancellationToken);
        if (client == null)
            return false;

        var personContact = await _personContactRepository.GetByPersonAndContactIdAsync(
            clientId, contactId, relationshipType, cancellationToken);
        
        if (personContact == null)
            return false;

        await _personContactRepository.DeleteAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    #endregion

    #region Provider Contacts

    public async Task<IEnumerable<PersonContactDto>> GetProviderContactsAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        var provider = await _personRepository.GetByIdAndRoleAsync(providerId, PROVIDER_ROLE_CODE, cancellationToken);
        if (provider == null)
            return Enumerable.Empty<PersonContactDto>();

        var contacts = await _personContactRepository.GetContactsByPersonIdAsync(providerId, cancellationToken);
        return contacts.Select(MapToDto);
    }

    public async Task<PersonContactDto?> GetProviderContactAsync(Guid providerId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var provider = await _personRepository.GetByIdAndRoleAsync(providerId, PROVIDER_ROLE_CODE, cancellationToken);
        if (provider == null)
            return null;

        var contact = await _personContactRepository.GetByPersonAndContactIdAsync(
            providerId, contactId, relationshipType, cancellationToken);
        
        return contact != null ? MapToDto(contact) : null;
    }

    public async Task<PersonContactDto> CreateProviderContactAsync(Guid providerId, CreatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        var provider = await _personRepository.GetByIdAndRoleAsync(providerId, PROVIDER_ROLE_CODE, cancellationToken);
        if (provider == null)
            throw new InvalidOperationException("Provider not found");

        var accountId = _currentUserService.GetCurrentAccountId();
        var personContact = new PersonContact
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            PersonId = providerId,
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
            providerId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        return created != null ? MapToDto(created) : throw new InvalidOperationException("Failed to create contact");
    }

    public async Task<PersonContactDto?> UpdateProviderContactAsync(Guid providerId, UpdatePersonContactDto dto, CancellationToken cancellationToken = default)
    {
        var provider = await _personRepository.GetByIdAndRoleAsync(providerId, PROVIDER_ROLE_CODE, cancellationToken);
        if (provider == null)
            return null;

        var personContact = await _personContactRepository.GetByPersonAndContactIdAsync(
            providerId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        if (personContact == null)
            return null;

        // Update fields
        UpdatePersonContactFields(personContact, dto);

        await _personContactRepository.UpdateAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _personContactRepository.GetByPersonAndContactIdAsync(
            providerId, dto.ContactId, dto.RelationshipType, cancellationToken);
        
        return updated != null ? MapToDto(updated) : null;
    }

    public async Task<bool> DeleteProviderContactAsync(Guid providerId, Guid contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var provider = await _personRepository.GetByIdAndRoleAsync(providerId, PROVIDER_ROLE_CODE, cancellationToken);
        if (provider == null)
            return false;

        var personContact = await _personContactRepository.GetByPersonAndContactIdAsync(
            providerId, contactId, relationshipType, cancellationToken);
        
        if (personContact == null)
            return false;

        await _personContactRepository.DeleteAsync(personContact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
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
        if (dto.LocationId.HasValue)
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

