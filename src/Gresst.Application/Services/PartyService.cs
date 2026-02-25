using Gresst.Application.DTOs;
using Gresst.Application.Services.Interfaces;
using Gresst.Domain.Entities;
using Gresst.Domain.Identity;
using Gresst.Domain.Interfaces;
using System.Linq.Expressions;
using System.Net;

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


    private PartyRelatedDto MapToDto(Party party)
    {
        var relations = party.Relations
                         .Select(r => r.ToString().ToLowerInvariant())
                         .ToList();

        return new PartyRelatedDto
        {
            Id = party.Id,
            Name = party.Name,
            PersonType = party.PersonType,
            DocumentType = party.DocumentType,
            DocumentNumber = party.DocumentNumber,
            CheckDigit = party.CheckDigit,
            Email = party.Email,
            Phone = party.Phone,
            Phone2 = party.Phone2,
            Address = party.Address,
            Location = party.Location,
            LocalityId = party.LocalityId,
            SignatureUrl = party.SignatureUrl,
            IsActive = party.IsActive,
            Relations = relations,
            Facilities = party.Facilities.Select(f => new FacilityDto
            {
                Id = f.Id,
                Name = f.Name,
                Address = f.Address,
                Location = f.Location,
                LocalityId = f.LocalityId,
                Phone = f.Phone,
                Email = f.Email,
                Reference = f.Reference,
                IsActive = f.IsActive,
                Types = f.Types,
                WasteTypes = f.WasteTypes.Select(w => new WasteTypeDto
                {
                    Id = w.Id,
                    Name = w.Name,
                }).ToList()
            }).ToList()
        };
    }

    public async Task<IEnumerable<PartyRelatedDto>> GetAllAsync(string? partyId = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return Enumerable.Empty<PartyRelatedDto>();

        var parties = await _partyRepository.GetAllAsync(partyId, cancellationToken);
        return parties.Select(MapToDto);
    }
    public async Task<IEnumerable<PartyRelatedDto>> GetAllWithDetailsAsync(string? partyId = null, CancellationToken cancellationToken = default)
    {
        var parties = await _partyRepository.GetAllWithDetailsAsync(partyId, cancellationToken);
        return parties.Select(MapToDto);
    }

    public async Task<IEnumerable<PartyRelatedDto>> FindAsync(Expression<Func<Party, bool>> predicate, string? partyId = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return Enumerable.Empty<PartyRelatedDto>();

        var parties = await _partyRepository.FindAsync(predicate, partyId, cancellationToken);
        return parties.Select(MapToDto);
    }

    public async Task<(IEnumerable<PartyRelatedDto> Items, string? Next)> FindPagedAsync(Expression<Func<Party, bool>>? predicate = null, string? partyId = null, int limit = 50, string? next = null, CancellationToken cancellationToken = default)
    {
        var take = Math.Clamp(limit, 1, 200);
        var pred = predicate ?? (p => true);
        (IEnumerable<Party> items, string? nextCursor) = await _partyRepository.FindPagedAsync(pred, partyId, take, next, cancellationToken);
        return (items.Select(MapToDto), nextCursor);
    }

    public async Task<PartyRelatedDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return null;

        var contact = await _partyRepository.GetByIdAsync(id, cancellationToken);
        
        return contact != null ? MapToDto(contact) : null;
    }

    public async Task<PartyRelatedDto?> CreateAsync(CreatePartyDto partyDto, PartyRelationType relation, string? partyId = null,  CancellationToken cancellationToken = default)
    {   
        var roles = new List<PartyRelationType> { relation };

        var party = new Party
        {
            Name = partyDto.Name,
            PersonType = partyDto.PersonType,
            DocumentNumber = partyDto.DocumentNumber,
            CheckDigit = partyDto.CheckDigit,
            DocumentType = partyDto.DocumentType,
            Address = partyDto.Address,
            Email = partyDto.Email,
            Phone = partyDto.Phone,
            Phone2 = partyDto.Phone2,
            Location = partyDto.Location,
            SignatureUrl = partyDto.SignatureUrl,
            LocalityId = partyDto.LocalityId,
            Relations = roles
        };

        await _partyRepository.AddAsync(party, partyId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(party);
    }
}

