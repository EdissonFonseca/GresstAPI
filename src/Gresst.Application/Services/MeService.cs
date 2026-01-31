using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for the current user's full context (user, account, person).
/// </summary>
public class MeService : IMeService
{
    private readonly IUserService _userService;
    private readonly IAccountRepository _accountRepository;
    private readonly IRepository<Person> _personRepository;

    public MeService(
        IUserService userService,
        IAccountRepository accountRepository,
        IRepository<Person> personRepository)
    {
        _userService = userService;
        _accountRepository = accountRepository;
        _personRepository = personRepository;
    }

    public async Task<MeResponseDto?> GetCurrentContextAsync(CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetCurrentUserAsync(cancellationToken);
        if (user == null)
            return null;

        var account = !string.IsNullOrEmpty(user.AccountId)
            ? await _accountRepository.GetByIdAsync(user.AccountId, cancellationToken)
            : null;

        // Person corresponding to the user: user's linked person first, then account's legal rep
        var personId = !string.IsNullOrEmpty(user.PersonId)
            ? user.PersonId
            : account?.PersonId;
        var person = !string.IsNullOrEmpty(personId)
            ? await _personRepository.GetByIdAsync(personId, cancellationToken)
            : null;

        return new MeResponseDto
        {
            Profile = user,
            Account = account != null ? MapAccount(account) : null,
            Person = person != null ? MapPerson(person) : null
        };
    }

    private static AccountSummaryDto MapAccount(Account account)
    {
        return new AccountSummaryDto
        {
            Id = account.Id,
            Name = account.Name,
            Code = account.Code,
            Role = account.Role.ToString(),
            Status = account.Status.ToString(),
            PersonId = account.PersonId ?? string.Empty,
            IsActive = account.IsActive
        };
    }

    private static PersonSummaryDto MapPerson(Person person)
    {
        return new PersonSummaryDto
        {
            Id = person.Id,
            Name = person.Name,
            DocumentNumber = person.DocumentNumber,
            Email = person.Email,
            Phone = person.Phone,
            Address = person.Address
        };
    }
}
