using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing Customers (Personas with Customer role)
/// Filters Personas by IdRol = "CL" (or similar customer role code)
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Role code for Customers in the database
    private const string CUSTOMER_ROLE_CODE = "CL"; // Adjust according to actual values in DB

    public CustomerService(
        IPersonRepository personRepository,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var persons = await _personRepository.GetCustomersAsync(cancellationToken);
        return persons.Select(MapToDto).ToList();
    }

    public async Task<CustomerDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAndRoleAsync(id, CUSTOMER_ROLE_CODE, cancellationToken);
        if (person == null)
            return null;

        return MapToDto(person);
    }

    public async Task<IEnumerable<CustomerDto>> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        var persons = await _personRepository.GetByRoleAsync(CUSTOMER_ROLE_CODE, cancellationToken);
        var filteredPersons = persons.Where(p => p.DocumentNumber == documentNumber);
        return filteredPersons.Select(MapToDto).ToList();
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var person = new Person
        {
            Id = string.Empty,
            AccountId = string.Empty,
            Name = dto.Name,
            DocumentNumber = dto.DocumentNumber,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            IsGenerator = dto.IsGenerator,
            IsCollector = dto.IsCollector,
            IsTransporter = dto.IsTransporter,
            IsReceiver = dto.IsReceiver,
            IsDisposer = dto.IsDisposer,
            IsTreater = dto.IsTreater,
            IsStorageProvider = dto.IsStorageProvider,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _personRepository.AddAsync(person, cancellationToken);
        await _personRepository.SetPersonRoleAsync(person.Id, CUSTOMER_ROLE_CODE, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(person);
    }

    public async Task<CustomerDto?> UpdateAsync(UpdateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAndRoleAsync(dto.Id, CUSTOMER_ROLE_CODE, cancellationToken);
        if (person == null)
            return null;

        if (!string.IsNullOrEmpty(dto.Name))
            person.Name = dto.Name;
        if (dto.DocumentNumber != null)
            person.DocumentNumber = dto.DocumentNumber;
        if (dto.Email != null)
            person.Email = dto.Email;
        if (dto.Phone != null)
            person.Phone = dto.Phone;
        if (dto.Address != null)
            person.Address = dto.Address;
        if (dto.IsGenerator.HasValue)
            person.IsGenerator = dto.IsGenerator.Value;
        if (dto.IsCollector.HasValue)
            person.IsCollector = dto.IsCollector.Value;
        if (dto.IsTransporter.HasValue)
            person.IsTransporter = dto.IsTransporter.Value;
        if (dto.IsReceiver.HasValue)
            person.IsReceiver = dto.IsReceiver.Value;
        if (dto.IsDisposer.HasValue)
            person.IsDisposer = dto.IsDisposer.Value;
        if (dto.IsTreater.HasValue)
            person.IsTreater = dto.IsTreater.Value;
        if (dto.IsStorageProvider.HasValue)
            person.IsStorageProvider = dto.IsStorageProvider.Value;
        if (dto.IsActive.HasValue)
            person.IsActive = dto.IsActive.Value;

        person.UpdatedAt = DateTime.UtcNow;

        await _personRepository.UpdateAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedPerson = await _personRepository.GetByIdAndRoleAsync(dto.Id, CUSTOMER_ROLE_CODE, cancellationToken);
        return updatedPerson != null ? MapToDto(updatedPerson) : null;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByIdAndRoleAsync(id, CUSTOMER_ROLE_CODE, cancellationToken);
        if (person == null)
            return false;

        await _personRepository.DeleteAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private CustomerDto MapToDto(Person person)
    {
        return new CustomerDto
        {
            Id = person.Id,
            Name = person.Name,
            DocumentNumber = person.DocumentNumber,
            Email = person.Email,
            Phone = person.Phone,
            Address = person.Address,
            AccountId = person.AccountId,
            IsGenerator = person.IsGenerator,
            IsCollector = person.IsCollector,
            IsTransporter = person.IsTransporter,
            IsReceiver = person.IsReceiver,
            IsDisposer = person.IsDisposer,
            IsTreater = person.IsTreater,
            IsStorageProvider = person.IsStorageProvider,
            CreatedAt = person.CreatedAt,
            UpdatedAt = person.UpdatedAt,
            IsActive = person.IsActive
        };
    }
}
