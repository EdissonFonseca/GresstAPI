using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing WasteClass and PersonWasteClass relationships
/// </summary>
public class WasteClassService : IWasteClassService
{
    private readonly IRepository<WasteClass> _wasteClassRepository;
    private readonly IRepository<PersonWasteClass> _personWasteClassRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    // Role code for Providers in the database
    private const string PROVIDER_ROLE_CODE = "PR";

    public WasteClassService(
        IRepository<WasteClass> wasteClassRepository,
        IRepository<PersonWasteClass> personWasteClassRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _wasteClassRepository = wasteClassRepository;
        _personWasteClassRepository = personWasteClassRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    // WasteClass CRUD
    public async Task<IEnumerable<WasteClassDto>> GetAllWasteClassesAsync(CancellationToken cancellationToken = default)
    {
        var wasteClasses = await _wasteClassRepository.GetAllAsync(cancellationToken);
        return wasteClasses.Select(MapWasteClassToDto).ToList();
    }

    public async Task<WasteClassDto?> GetWasteClassByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var wasteClass = await _wasteClassRepository.GetByIdAsync(id, cancellationToken);
        if (wasteClass == null)
            return null;

        return MapWasteClassToDto(wasteClass);
    }

    public async Task<WasteClassDto> CreateWasteClassAsync(CreateWasteClassDto dto, CancellationToken cancellationToken = default)
    {
        var wasteClass = new WasteClass
        {
            Id = Guid.NewGuid(),
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            ClassificationId = dto.ClassificationId,
            IsHazardous = dto.IsHazardous,
            RequiresSpecialHandling = dto.RequiresSpecialHandling,
            PhysicalState = dto.PhysicalState,
            TreatmentId = dto.TreatmentId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _wasteClassRepository.AddAsync(wasteClass, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapWasteClassToDto(wasteClass);
    }

    public async Task<WasteClassDto?> UpdateWasteClassAsync(UpdateWasteClassDto dto, CancellationToken cancellationToken = default)
    {
        var wasteClass = await _wasteClassRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (wasteClass == null)
            return null;

        if (!string.IsNullOrEmpty(dto.Name))
            wasteClass.Name = dto.Name;
        if (dto.Description != null)
            wasteClass.Description = dto.Description;
        if (dto.ClassificationId.HasValue)
            wasteClass.ClassificationId = dto.ClassificationId;
        if (dto.IsHazardous.HasValue)
            wasteClass.IsHazardous = dto.IsHazardous.Value;
        if (dto.RequiresSpecialHandling.HasValue)
            wasteClass.RequiresSpecialHandling = dto.RequiresSpecialHandling.Value;
        if (dto.PhysicalState != null)
            wasteClass.PhysicalState = dto.PhysicalState;
        if (dto.TreatmentId.HasValue)
            wasteClass.TreatmentId = dto.TreatmentId;
        if (dto.IsActive.HasValue)
            wasteClass.IsActive = dto.IsActive.Value;

        wasteClass.UpdatedAt = DateTime.UtcNow;

        await _wasteClassRepository.UpdateAsync(wasteClass, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapWasteClassToDto(wasteClass);
    }

    public async Task<bool> DeleteWasteClassAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var wasteClass = await _wasteClassRepository.GetByIdAsync(id, cancellationToken);
        if (wasteClass == null)
            return false;

        await _wasteClassRepository.DeleteAsync(wasteClass, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    // PersonWasteClass - Account Person
    private async Task<Guid> GetAccountPersonIdAsync(CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || account.PersonId == Guid.Empty)
            throw new InvalidOperationException("Account or Account Person not found");
        return account.PersonId;
    }

    public async Task<IEnumerable<PersonWasteClassDto>> GetAccountPersonWasteClassesAsync(CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await GetPersonWasteClassesAsync(accountPersonId, cancellationToken);
    }

    public async Task<PersonWasteClassDto> CreateAccountPersonWasteClassAsync(CreatePersonWasteClassDto dto, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await CreatePersonWasteClassAsync(accountPersonId, dto, cancellationToken);
    }

    public async Task<PersonWasteClassDto?> UpdateAccountPersonWasteClassAsync(UpdatePersonWasteClassDto dto, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        dto.PersonId = accountPersonId;
        return await UpdatePersonWasteClassAsync(dto, cancellationToken);
    }

    public async Task<bool> DeleteAccountPersonWasteClassAsync(Guid wasteClassId, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await DeletePersonWasteClassAsync(accountPersonId, wasteClassId, cancellationToken);
    }

    // PersonWasteClass - Provider
    public async Task<IEnumerable<PersonWasteClassDto>> GetProviderWasteClassesAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        return await GetPersonWasteClassesAsync(providerId, cancellationToken);
    }

    public async Task<PersonWasteClassDto> CreateProviderWasteClassAsync(Guid providerId, CreatePersonWasteClassDto dto, CancellationToken cancellationToken = default)
    {
        return await CreatePersonWasteClassAsync(providerId, dto, cancellationToken);
    }

    public async Task<PersonWasteClassDto?> UpdateProviderWasteClassAsync(Guid providerId, UpdatePersonWasteClassDto dto, CancellationToken cancellationToken = default)
    {
        dto.PersonId = providerId;
        return await UpdatePersonWasteClassAsync(dto, cancellationToken);
    }

    public async Task<bool> DeleteProviderWasteClassAsync(Guid providerId, Guid wasteClassId, CancellationToken cancellationToken = default)
    {
        return await DeletePersonWasteClassAsync(providerId, wasteClassId, cancellationToken);
    }

    // Helper methods
    private async Task<IEnumerable<PersonWasteClassDto>> GetPersonWasteClassesAsync(Guid personId, CancellationToken cancellationToken)
    {
        var personWasteClasses = await _personWasteClassRepository.FindAsync(
            pwc => pwc.PersonId == personId,
            cancellationToken);
        
        return personWasteClasses.Select(MapPersonWasteClassToDto).ToList();
    }

    private async Task<PersonWasteClassDto> CreatePersonWasteClassAsync(Guid personId, CreatePersonWasteClassDto dto, CancellationToken cancellationToken)
    {
        var personWasteClass = new PersonWasteClass
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            WasteClassId = dto.WasteClassId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _personWasteClassRepository.AddAsync(personWasteClass, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapPersonWasteClassToDtoWithNamesAsync(personWasteClass, cancellationToken);
    }

    private async Task<PersonWasteClassDto?> UpdatePersonWasteClassAsync(UpdatePersonWasteClassDto dto, CancellationToken cancellationToken)
    {
        var personWasteClasses = await _personWasteClassRepository.FindAsync(
            pwc => pwc.PersonId == dto.PersonId && pwc.WasteClassId == dto.WasteClassId,
            cancellationToken);
        
        var personWasteClass = personWasteClasses.FirstOrDefault();
        if (personWasteClass == null)
            return null;

        if (dto.IsActive.HasValue)
            personWasteClass.IsActive = dto.IsActive.Value;

        personWasteClass.UpdatedAt = DateTime.UtcNow;

        await _personWasteClassRepository.UpdateAsync(personWasteClass, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapPersonWasteClassToDtoWithNamesAsync(personWasteClass, cancellationToken);
    }

    private async Task<bool> DeletePersonWasteClassAsync(Guid personId, Guid wasteClassId, CancellationToken cancellationToken)
    {
        var personWasteClasses = await _personWasteClassRepository.FindAsync(
            pwc => pwc.PersonId == personId && pwc.WasteClassId == wasteClassId,
            cancellationToken);
        
        var personWasteClass = personWasteClasses.FirstOrDefault();
        if (personWasteClass == null)
            return false;

        await _personWasteClassRepository.DeleteAsync(personWasteClass, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private WasteClassDto MapWasteClassToDto(WasteClass wasteClass)
    {
        return new WasteClassDto
        {
            Id = wasteClass.Id,
            Code = wasteClass.Code,
            Name = wasteClass.Name,
            Description = wasteClass.Description,
            ClassificationId = wasteClass.ClassificationId,
            ClassificationName = wasteClass.Classification?.Name,
            IsHazardous = wasteClass.IsHazardous,
            RequiresSpecialHandling = wasteClass.RequiresSpecialHandling,
            PhysicalState = wasteClass.PhysicalState,
            TreatmentId = wasteClass.TreatmentId,
            TreatmentName = wasteClass.Treatment?.Name,
            IsActive = wasteClass.IsActive,
            CreatedAt = wasteClass.CreatedAt,
            UpdatedAt = wasteClass.UpdatedAt
        };
    }

    private PersonWasteClassDto MapPersonWasteClassToDto(PersonWasteClass personWasteClass)
    {
        return new PersonWasteClassDto
        {
            Id = personWasteClass.Id,
            PersonId = personWasteClass.PersonId,
            PersonName = personWasteClass.Person?.Name,
            WasteClassId = personWasteClass.WasteClassId,
            WasteClassName = personWasteClass.WasteClass?.Name,
            IsActive = personWasteClass.IsActive,
            CreatedAt = personWasteClass.CreatedAt,
            UpdatedAt = personWasteClass.UpdatedAt
        };
    }

    private async Task<PersonWasteClassDto> MapPersonWasteClassToDtoWithNamesAsync(PersonWasteClass personWasteClass, CancellationToken cancellationToken)
    {
        // Reload with includes to get names
        var all = await _personWasteClassRepository.GetAllAsync(cancellationToken);
        var reloaded = all.FirstOrDefault(pwc => 
            pwc.PersonId == personWasteClass.PersonId && 
            pwc.WasteClassId == personWasteClass.WasteClassId);
        
        return MapPersonWasteClassToDto(reloaded ?? personWasteClass);
    }
}

