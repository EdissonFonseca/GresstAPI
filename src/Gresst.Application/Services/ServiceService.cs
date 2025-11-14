using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing Services and PersonService relationships
/// </summary>
public class ServiceService : IServiceService
{
    private readonly IRepository<Service> _serviceRepository;
    private readonly IRepository<PersonService> _personServiceRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    // Role code for Providers in the database
    private const string PROVIDER_ROLE_CODE = "PR";

    public ServiceService(
        IRepository<Service> serviceRepository,
        IRepository<PersonService> personServiceRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _serviceRepository = serviceRepository;
        _personServiceRepository = personServiceRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    // Service CRUD
    public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync(CancellationToken cancellationToken = default)
    {
        var services = await _serviceRepository.GetAllAsync(cancellationToken);
        return services.Select(MapServiceToDto).ToList();
    }

    public async Task<ServiceDto?> GetServiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
        if (service == null)
            return null;

        return MapServiceToDto(service);
    }

    public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto, CancellationToken cancellationToken = default)
    {
        var service = new Service
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            CategoryCode = dto.CategoryCode,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _serviceRepository.AddAsync(service, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapServiceToDto(service);
    }

    public async Task<ServiceDto?> UpdateServiceAsync(UpdateServiceDto dto, CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (service == null)
            return null;

        if (!string.IsNullOrEmpty(dto.Name))
            service.Name = dto.Name;
        if (dto.Description != null)
            service.Description = dto.Description;
        if (dto.CategoryCode != null)
            service.CategoryCode = dto.CategoryCode;
        if (dto.IsActive.HasValue)
            service.IsActive = dto.IsActive.Value;

        service.UpdatedAt = DateTime.UtcNow;

        await _serviceRepository.UpdateAsync(service, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapServiceToDto(service);
    }

    public async Task<bool> DeleteServiceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(id, cancellationToken);
        if (service == null)
            return false;

        await _serviceRepository.DeleteAsync(service, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    // PersonService - Account Person
    private async Task<Guid> GetAccountPersonIdAsync(CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || account.PersonId == Guid.Empty)
            throw new InvalidOperationException("Account or Account Person not found");
        return account.PersonId;
    }

    public async Task<IEnumerable<PersonServiceDto>> GetAccountPersonServicesAsync(CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await GetPersonServicesAsync(accountPersonId, cancellationToken);
    }

    public async Task<PersonServiceDto> CreateAccountPersonServiceAsync(CreatePersonServiceDto dto, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await CreatePersonServiceAsync(accountPersonId, dto, cancellationToken);
    }

    public async Task<PersonServiceDto?> UpdateAccountPersonServiceAsync(UpdatePersonServiceDto dto, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        dto.PersonId = accountPersonId;
        return await UpdatePersonServiceAsync(dto, cancellationToken);
    }

    public async Task<bool> DeleteAccountPersonServiceAsync(Guid serviceId, DateTime startDate, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await DeletePersonServiceAsync(accountPersonId, serviceId, startDate, cancellationToken);
    }

    // PersonService - Provider
    public async Task<IEnumerable<PersonServiceDto>> GetProviderServicesAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        return await GetPersonServicesAsync(providerId, cancellationToken);
    }

    public async Task<PersonServiceDto> CreateProviderServiceAsync(Guid providerId, CreatePersonServiceDto dto, CancellationToken cancellationToken = default)
    {
        return await CreatePersonServiceAsync(providerId, dto, cancellationToken);
    }

    public async Task<PersonServiceDto?> UpdateProviderServiceAsync(Guid providerId, UpdatePersonServiceDto dto, CancellationToken cancellationToken = default)
    {
        dto.PersonId = providerId;
        return await UpdatePersonServiceAsync(dto, cancellationToken);
    }

    public async Task<bool> DeleteProviderServiceAsync(Guid providerId, Guid serviceId, DateTime startDate, CancellationToken cancellationToken = default)
    {
        return await DeletePersonServiceAsync(providerId, serviceId, startDate, cancellationToken);
    }

    // Helper methods
    private async Task<IEnumerable<PersonServiceDto>> GetPersonServicesAsync(Guid personId, CancellationToken cancellationToken)
    {
        var personServices = await _personServiceRepository.FindAsync(
            ps => ps.PersonId == personId,
            cancellationToken);
        
        return personServices.Select(MapPersonServiceToDto).ToList();
    }

    private async Task<PersonServiceDto> CreatePersonServiceAsync(Guid personId, CreatePersonServiceDto dto, CancellationToken cancellationToken)
    {
        var personService = new PersonService
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            ServiceId = dto.ServiceId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _personServiceRepository.AddAsync(personService, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapPersonServiceToDtoWithNamesAsync(personService, cancellationToken);
    }

    private async Task<PersonServiceDto?> UpdatePersonServiceAsync(UpdatePersonServiceDto dto, CancellationToken cancellationToken)
    {
        var personServices = await _personServiceRepository.FindAsync(
            ps => ps.PersonId == dto.PersonId && ps.ServiceId == dto.ServiceId && ps.StartDate == dto.StartDate,
            cancellationToken);
        
        var personService = personServices.FirstOrDefault();
        if (personService == null)
            return null;

        if (dto.EndDate.HasValue)
            personService.EndDate = dto.EndDate;
        if (dto.IsActive.HasValue)
            personService.IsActive = dto.IsActive.Value;

        personService.UpdatedAt = DateTime.UtcNow;

        await _personServiceRepository.UpdateAsync(personService, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapPersonServiceToDtoWithNamesAsync(personService, cancellationToken);
    }

    private async Task<bool> DeletePersonServiceAsync(Guid personId, Guid serviceId, DateTime startDate, CancellationToken cancellationToken)
    {
        var personServices = await _personServiceRepository.FindAsync(
            ps => ps.PersonId == personId && ps.ServiceId == serviceId && ps.StartDate == startDate,
            cancellationToken);
        
        var personService = personServices.FirstOrDefault();
        if (personService == null)
            return false;

        await _personServiceRepository.DeleteAsync(personService, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private ServiceDto MapServiceToDto(Service service)
    {
        return new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            CategoryCode = service.CategoryCode,
            IsActive = service.IsActive,
            CreatedAt = service.CreatedAt,
            UpdatedAt = service.UpdatedAt
        };
    }

    private PersonServiceDto MapPersonServiceToDto(PersonService personService)
    {
        return new PersonServiceDto
        {
            Id = personService.Id,
            PersonId = personService.PersonId,
            PersonName = personService.Person?.Name,
            ServiceId = personService.ServiceId,
            ServiceName = personService.Service?.Name,
            StartDate = personService.StartDate,
            EndDate = personService.EndDate,
            IsActive = personService.IsActive,
            CreatedAt = personService.CreatedAt,
            UpdatedAt = personService.UpdatedAt
        };
    }

    private async Task<PersonServiceDto> MapPersonServiceToDtoWithNamesAsync(PersonService personService, CancellationToken cancellationToken)
    {
        // Reload with includes to get names
        var all = await _personServiceRepository.GetAllAsync(cancellationToken);
        var reloaded = all.FirstOrDefault(ps => 
            ps.PersonId == personService.PersonId && 
            ps.ServiceId == personService.ServiceId && 
            ps.StartDate == personService.StartDate);
        
        return MapPersonServiceToDto(reloaded ?? personService);
    }
}

