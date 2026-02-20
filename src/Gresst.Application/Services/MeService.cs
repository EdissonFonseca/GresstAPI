using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Identity;
using Gresst.Domain.Interfaces;

namespace Gresst.Application.Services;

/// <summary>
/// Service for the current user's full context (user, account, person, roles, permissions).
/// </summary>
public class MeService : IMeService
{
    private readonly IUserService _userService;
    private readonly IRepository<Account> _accountRepository;
    private readonly IRepository<Party> _partyRepository;
    private readonly IAuthorizationService _authorizationService;

    public MeService(
        IUserService userService,
        IRepository<Account> accountRepository,
        IRepository<Party> partyRepository,
        IAuthorizationService authorizationService)
    {
        _userService = userService;
        _accountRepository = accountRepository;
        _partyRepository = partyRepository;
        _authorizationService = authorizationService;
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
        var partyId = !string.IsNullOrEmpty(user.PartyId)
            ? user.PartyId
            : account?.PartyId;
        var party = !string.IsNullOrEmpty(partyId)
            ? await _partyRepository.GetByIdAsync(partyId, cancellationToken)
            : null;

        return new MeResponseDto
        {
            Profile = MapToMeProfile(user),
            Account = account != null ? MapAccount(account) : null,
            Party = party != null ? MapParty(party) : null,
            Roles = user.Roles ?? Array.Empty<string>(),
        };
    }

    public async Task<IEnumerable<UserPermissionDto>> GetCurrentUserPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetCurrentUserAsync(cancellationToken);
        if (user == null)
            return Array.Empty<UserPermissionDto>();

        return await _authorizationService.GetUserPermissionsAsync(user.Id, cancellationToken);
    }

    private static MeProfileDto MapToMeProfile(UserDto user)
    {
        return new MeProfileDto
        {
            Id = user.Id,
            AccountId = user.AccountId,
            FirstName = user.Name ?? string.Empty,
            LastName = user.LastName,
            Email = user.Email,
            Status = user.Status,
            PartyId = user.PartyId,
            LastAccess = user.LastAccess,
            CreatedAt = user.CreatedAt
        };
    }

    private static AccountSummaryDto MapAccount(Account account)
    {
        return new AccountSummaryDto
        {
            Id = account.Id,
            Name = account.Name,
            Role = account.Role.ToString(),
            Status = account.Status.ToString(),
            PartyId = account.PartyId ?? string.Empty,
            IsActive = account.IsActive
        };
    }

    private static PartySummaryDto MapParty(Party party)
    {
        return new PartySummaryDto
        {
            Id = party.Id,
            Name = party.Name,
            DocumentNumber = party.DocumentNumber,
            Email = party.Email,
            Phone = party.Phone,
            Address = party.Address
        };
    }
}
