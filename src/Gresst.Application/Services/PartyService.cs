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
    private readonly IRepository<Party> _partyRepository;
    private readonly IRepository<Account> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public PartyService(
        IRepository<Party> partyRepository,
        IRepository<Account> accountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _partyRepository = partyRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<(IEnumerable<PartyDto> Items, string? Next)> FindPagedAsync(
        string? ownerId = null,
        PartyRelationType? role = null,
        bool? isActive = null,
        string? search = null,
        int limit = 50,
        string? next = null,
        CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null || string.IsNullOrEmpty(account.PartyId))
            return (Enumerable.Empty<PartyDto>(), null);

        // Build predicate similar to controller usage
        Expression<Func<Party, bool>> predicate = p => true;
        if (!string.IsNullOrEmpty(ownerId))
            predicate = AndAlso(predicate, (Party p) => p.AccountId == ownerId);
        if (role.HasValue)
            predicate = AndAlso(predicate, (Party p) => p.Roles.Contains(role.Value));
        if (isActive.HasValue)
            predicate = AndAlso(predicate, (Party p) => p.IsActive == isActive.Value);
        if (!string.IsNullOrEmpty(search))
        {
            var s = search.Trim();
            predicate = AndAlso(predicate, (Party p) => p.Name != null && p.Name.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        var all = await _partyRepository.FindAsync(predicate, cancellationToken);
        var ordered = all.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id).ToList();
        var take = Math.Clamp(limit, 1, 200);

        var startIndex = 0;
        if (!string.IsNullOrEmpty(next))
        {
            var idx = ordered.FindIndex(p => p.Id == next);
            if (idx >= 0) startIndex = idx + 1;
        }

        var page = ordered.Skip(startIndex).Take(take).Select(MapToDto).ToList();
        string? nextCursor = null;
        if (startIndex + page.Count < ordered.Count)
            nextCursor = page.LastOrDefault()?.Id;

        return (page, nextCursor);
    }

    // Local helper to combine expressions without depending on API layer
    private static Expression<Func<T, bool>> AndAlso<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        if (first == null) return second;
        if (second == null) return first;

        var parameter = Expression.Parameter(typeof(T));
        var left = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var right = ReplaceParameter(second.Body, second.Parameters[0], parameter);
        var body = Expression.AndAlso(left, right);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression toReplace, ParameterExpression replaceWith)
    {
        return new ParameterReplacer(toReplace, replaceWith).Visit(expression) ?? expression;
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ParameterReplacer(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == _from) return _to;
            return base.VisitParameter(node);
        }
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

