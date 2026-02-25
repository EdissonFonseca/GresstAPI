using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using System.Linq.Expressions;

namespace Gresst.Application.Services.Interfaces;

public interface IPartyService
{
    Task<IEnumerable<PartyRelatedDto>> GetAllAsync(string? partyId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<PartyRelatedDto>> GetAllWithDetailsAsync(string? partyId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<PartyRelatedDto>> FindAsync(Expression<Func<Party, bool>> predicate, string? partyId = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<PartyRelatedDto> Items, string? Next)> FindPagedAsync(Expression<Func<Party, bool>>? predicate = null, string? partyId = null, int limit = 50, string? next = null, CancellationToken cancellationToken = default);
    Task<PartyRelatedDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}

