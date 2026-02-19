using Gresst.Application.DTOs;
using Gresst.Application.Queries;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using System.Linq.Expressions;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing Partys
/// Handles contacts for Account Party, Customers, and Providers
/// </summary>
public class PartyService : IPartyService
{
    private readonly IRepository<Party> _partyRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    // Role codes
    private const string CLIENT_ROLE_CODE = "CL";
    private const string PROVIDER_ROLE_CODE = "PR";

    public PartyService(
        IRepository<Party> partyRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _partyRepository = partyRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    private PartyDto MapToDto(Party party)
    {
        // Mapeo básico desde Domain Material al DTO
        // Los campos adicionales (ServicePrice, PurchasePrice, etc.) se pueden agregar
        // al Domain Material en el futuro o mapear directamente desde el Repository
        return new PartyDto
        {
            Id = party.Id,
            Name = party.Name,
            IsActive = party.IsActive,
            CreatedAt = party.CreatedAt,
            UpdatedAt = party.UpdatedAt
        };
    }

    public async Task<IEnumerable<PartyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return Enumerable.Empty<PartyDto>();

        var parties = await _partyRepository.GetAllAsync(cancellationToken);
        return parties.Select(MapToDto);
    }

    public async Task<IEnumerable<PartyDto>> FindAsync(Expression<Func<Party, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return Enumerable.Empty<PartyDto>();

        var parties = await _partyRepository.FindAsync(predicate, cancellationToken);
        return parties.Select(MapToDto);
    }

    public async Task<PartyDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return null;

        var contact = await _partyRepository.GetByIdAsync(id, cancellationToken);
        
        return contact != null ? MapToDto(contact) : null;
    }
}

