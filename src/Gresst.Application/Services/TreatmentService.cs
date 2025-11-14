using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing Treatment and PersonTreatment relationships
/// </summary>
public class TreatmentService : ITreatmentService
{
    private readonly IRepository<Treatment> _treatmentRepository;
    private readonly IRepository<PersonTreatment> _personTreatmentRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    // Role code for Providers in the database
    private const string PROVIDER_ROLE_CODE = "PR";

    public TreatmentService(
        IRepository<Treatment> treatmentRepository,
        IRepository<PersonTreatment> personTreatmentRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _treatmentRepository = treatmentRepository;
        _personTreatmentRepository = personTreatmentRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    // Treatment CRUD
    public async Task<IEnumerable<TreatmentDto>> GetAllTreatmentsAsync(CancellationToken cancellationToken = default)
    {
        var treatments = await _treatmentRepository.GetAllAsync(cancellationToken);
        return treatments.Select(MapTreatmentToDto).ToList();
    }

    public async Task<TreatmentDto?> GetTreatmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var treatment = await _treatmentRepository.GetByIdAsync(id, cancellationToken);
        if (treatment == null)
            return null;

        return MapTreatmentToDto(treatment);
    }

    public async Task<TreatmentDto> CreateTreatmentAsync(CreateTreatmentDto dto, CancellationToken cancellationToken = default)
    {
        var treatment = new Treatment
        {
            Id = Guid.NewGuid(),
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            ServiceId = dto.ServiceId,
            ProcessDescription = dto.ProcessDescription,
            EstimatedDuration = dto.EstimatedDuration,
            ApplicableWasteClasses = dto.ApplicableWasteClasses,
            ProducesNewWaste = dto.ProducesNewWaste,
            ResultingWasteClasses = dto.ResultingWasteClasses,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _treatmentRepository.AddAsync(treatment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapTreatmentToDto(treatment);
    }

    public async Task<TreatmentDto?> UpdateTreatmentAsync(UpdateTreatmentDto dto, CancellationToken cancellationToken = default)
    {
        var treatment = await _treatmentRepository.GetByIdAsync(dto.Id, cancellationToken);
        if (treatment == null)
            return null;

        if (!string.IsNullOrEmpty(dto.Name))
            treatment.Name = dto.Name;
        if (dto.Description != null)
            treatment.Description = dto.Description;
        if (!string.IsNullOrEmpty(dto.Category))
            treatment.Category = dto.Category;
        if (dto.ServiceId.HasValue)
            treatment.ServiceId = dto.ServiceId.Value;
        if (dto.ProcessDescription != null)
            treatment.ProcessDescription = dto.ProcessDescription;
        if (dto.EstimatedDuration.HasValue)
            treatment.EstimatedDuration = dto.EstimatedDuration;
        if (dto.ApplicableWasteClasses != null)
            treatment.ApplicableWasteClasses = dto.ApplicableWasteClasses;
        if (dto.ProducesNewWaste.HasValue)
            treatment.ProducesNewWaste = dto.ProducesNewWaste.Value;
        if (dto.ResultingWasteClasses != null)
            treatment.ResultingWasteClasses = dto.ResultingWasteClasses;
        if (dto.IsActive.HasValue)
            treatment.IsActive = dto.IsActive.Value;

        treatment.UpdatedAt = DateTime.UtcNow;

        await _treatmentRepository.UpdateAsync(treatment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapTreatmentToDto(treatment);
    }

    public async Task<bool> DeleteTreatmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var treatment = await _treatmentRepository.GetByIdAsync(id, cancellationToken);
        if (treatment == null)
            return false;

        await _treatmentRepository.DeleteAsync(treatment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    // PersonTreatment - Account Person
    private async Task<Guid> GetAccountPersonIdAsync(CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || account.PersonId == Guid.Empty)
            throw new InvalidOperationException("Account or Account Person not found");
        return account.PersonId;
    }

    public async Task<IEnumerable<PersonTreatmentDto>> GetAccountPersonTreatmentsAsync(CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await GetPersonTreatmentsAsync(accountPersonId, cancellationToken);
    }

    public async Task<PersonTreatmentDto> CreateAccountPersonTreatmentAsync(CreatePersonTreatmentDto dto, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await CreatePersonTreatmentAsync(accountPersonId, dto, cancellationToken);
    }

    public async Task<PersonTreatmentDto?> UpdateAccountPersonTreatmentAsync(UpdatePersonTreatmentDto dto, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        dto.PersonId = accountPersonId;
        return await UpdatePersonTreatmentAsync(dto, cancellationToken);
    }

    public async Task<bool> DeleteAccountPersonTreatmentAsync(Guid treatmentId, CancellationToken cancellationToken = default)
    {
        var accountPersonId = await GetAccountPersonIdAsync(cancellationToken);
        return await DeletePersonTreatmentAsync(accountPersonId, treatmentId, cancellationToken);
    }

    // PersonTreatment - Provider
    public async Task<IEnumerable<PersonTreatmentDto>> GetProviderTreatmentsAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        return await GetPersonTreatmentsAsync(providerId, cancellationToken);
    }

    public async Task<PersonTreatmentDto> CreateProviderTreatmentAsync(Guid providerId, CreatePersonTreatmentDto dto, CancellationToken cancellationToken = default)
    {
        return await CreatePersonTreatmentAsync(providerId, dto, cancellationToken);
    }

    public async Task<PersonTreatmentDto?> UpdateProviderTreatmentAsync(Guid providerId, UpdatePersonTreatmentDto dto, CancellationToken cancellationToken = default)
    {
        dto.PersonId = providerId;
        return await UpdatePersonTreatmentAsync(dto, cancellationToken);
    }

    public async Task<bool> DeleteProviderTreatmentAsync(Guid providerId, Guid treatmentId, CancellationToken cancellationToken = default)
    {
        return await DeletePersonTreatmentAsync(providerId, treatmentId, cancellationToken);
    }

    // Helper methods
    private async Task<IEnumerable<PersonTreatmentDto>> GetPersonTreatmentsAsync(Guid personId, CancellationToken cancellationToken)
    {
        var personTreatments = await _personTreatmentRepository.FindAsync(
            pt => pt.PersonId == personId,
            cancellationToken);
        
        return personTreatments.Select(MapPersonTreatmentToDto).ToList();
    }

    private async Task<PersonTreatmentDto> CreatePersonTreatmentAsync(Guid personId, CreatePersonTreatmentDto dto, CancellationToken cancellationToken)
    {
        var personTreatment = new PersonTreatment
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            TreatmentId = dto.TreatmentId,
            IsManaged = dto.IsManaged,
            CanTransfer = dto.CanTransfer,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _personTreatmentRepository.AddAsync(personTreatment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapPersonTreatmentToDtoWithNamesAsync(personTreatment, cancellationToken);
    }

    private async Task<PersonTreatmentDto?> UpdatePersonTreatmentAsync(UpdatePersonTreatmentDto dto, CancellationToken cancellationToken)
    {
        var personTreatments = await _personTreatmentRepository.FindAsync(
            pt => pt.PersonId == dto.PersonId && pt.TreatmentId == dto.TreatmentId,
            cancellationToken);
        
        var personTreatment = personTreatments.FirstOrDefault();
        if (personTreatment == null)
            return null;

        if (dto.IsManaged.HasValue)
            personTreatment.IsManaged = dto.IsManaged.Value;
        if (dto.CanTransfer.HasValue)
            personTreatment.CanTransfer = dto.CanTransfer.Value;
        if (dto.IsActive.HasValue)
            personTreatment.IsActive = dto.IsActive.Value;

        personTreatment.UpdatedAt = DateTime.UtcNow;

        await _personTreatmentRepository.UpdateAsync(personTreatment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapPersonTreatmentToDtoWithNamesAsync(personTreatment, cancellationToken);
    }

    private async Task<bool> DeletePersonTreatmentAsync(Guid personId, Guid treatmentId, CancellationToken cancellationToken)
    {
        var personTreatments = await _personTreatmentRepository.FindAsync(
            pt => pt.PersonId == personId && pt.TreatmentId == treatmentId,
            cancellationToken);
        
        var personTreatment = personTreatments.FirstOrDefault();
        if (personTreatment == null)
            return false;

        await _personTreatmentRepository.DeleteAsync(personTreatment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    private TreatmentDto MapTreatmentToDto(Treatment treatment)
    {
        return new TreatmentDto
        {
            Id = treatment.Id,
            Code = treatment.Code,
            Name = treatment.Name,
            Description = treatment.Description,
            Category = treatment.Category,
            ServiceId = treatment.ServiceId,
            ServiceName = treatment.Service?.Name,
            ProcessDescription = treatment.ProcessDescription,
            EstimatedDuration = treatment.EstimatedDuration,
            ApplicableWasteClasses = treatment.ApplicableWasteClasses,
            ProducesNewWaste = treatment.ProducesNewWaste,
            ResultingWasteClasses = treatment.ResultingWasteClasses,
            WasteClassId = treatment.WasteClass?.Id,
            WasteClassName = treatment.WasteClass?.Name,
            IsActive = treatment.IsActive,
            CreatedAt = treatment.CreatedAt,
            UpdatedAt = treatment.UpdatedAt
        };
    }

    private PersonTreatmentDto MapPersonTreatmentToDto(PersonTreatment personTreatment)
    {
        return new PersonTreatmentDto
        {
            Id = personTreatment.Id,
            PersonId = personTreatment.PersonId,
            PersonName = personTreatment.Person?.Name,
            TreatmentId = personTreatment.TreatmentId,
            TreatmentName = personTreatment.Treatment?.Name,
            IsManaged = personTreatment.IsManaged,
            CanTransfer = personTreatment.CanTransfer,
            IsActive = personTreatment.IsActive,
            CreatedAt = personTreatment.CreatedAt,
            UpdatedAt = personTreatment.UpdatedAt
        };
    }

    private async Task<PersonTreatmentDto> MapPersonTreatmentToDtoWithNamesAsync(PersonTreatment personTreatment, CancellationToken cancellationToken)
    {
        // Reload with includes to get names
        var all = await _personTreatmentRepository.GetAllAsync(cancellationToken);
        var reloaded = all.FirstOrDefault(pt => 
            pt.PersonId == personTreatment.PersonId && 
            pt.TreatmentId == personTreatment.TreatmentId);
        
        return MapPersonTreatmentToDto(reloaded ?? personTreatment);
    }
}

