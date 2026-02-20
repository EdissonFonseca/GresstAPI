using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using Gresst.Domain.Identity;
using Gresst.Domain.Interfaces;
using System.Linq.Expressions;

namespace Gresst.Application.Services;

/// <summary>
/// Service for managing Partys
/// Handles contacts for Account Party, Customers, and Providers
/// </summary>
public class PartyService : IPartyService
{
    private readonly IPartyRepository<Party> _partyRepository;
    private readonly IRepository<Account> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public PartyService(IPartyRepository<Party> partyRepository,
        IRepository<Account> accountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _partyRepository = partyRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    // Local helper to combine expressions without depending on API layer


    private PartyDto MapToDto(Party party)
    {
        // Mapeo básico desde Domain Material al DTO
        // Los campos adicionales (ServicePrice, PurchasePrice, etc.) se pueden agregar
        // al Domain Material en el futuro o mapear directamente desde el Repository
        return new PartyDto
        {
            Id = party.Id,
            Name = party.Name,
            Email = party.Email,
            Phone = party.Phone,
            Phone2 = party.Phone2,
            Address = party.Address,
            DocumentNumber = party.DocumentNumber,
            LocationId = party.LocationId,
            IsActive = party.IsActive
        };
    }

    public async Task<IEnumerable<PartyDto>> GetAllAsync(string? partyId = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return Enumerable.Empty<PartyDto>();

        var parties = await _partyRepository.GetAllAsync(partyId, cancellationToken);
        return parties.Select(MapToDto);
    }

    public async Task<IEnumerable<PartyDto>> FindAsync(Expression<Func<Party, bool>> predicate, string? partyId = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return Enumerable.Empty<PartyDto>();

        var parties = await _partyRepository.FindAsync(predicate, partyId, cancellationToken);
        return parties.Select(MapToDto);
    }

    public async Task<(IEnumerable<PartyDto> Items, string? Next)> FindPagedAsync(Expression<Func<Party, bool>>? predicate = null, string? partyId = null, int limit = 50, string? next = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return (Enumerable.Empty<PartyDto>(), null);

        var take = Math.Clamp(limit, 1, 200);
        var pred = predicate ?? (p => true);
        (IEnumerable<Party> items, string? nextCursor) = await _partyRepository.FindPagedAsync(pred, partyId, take, next, cancellationToken);
        return (items.Select(MapToDto), nextCursor);
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

