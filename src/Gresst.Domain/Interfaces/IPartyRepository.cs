using Gresst.Domain.Common;
using System.Linq.Expressions;

namespace Gresst.Domain.Interfaces
{
    public interface IPartyRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(string? partyId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string? partyId = null, CancellationToken cancellationToken = default);
        Task<(IEnumerable<T> Items, string? Next)> FindPagedAsync(Expression<Func<T, bool>> predicate, string? partyId = null, int limit = 50, string? next = null, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, string? partyId = null, CancellationToken cancellationToken = default);

        Task<T> AddAsync(T entity, string? partyId, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, string? partyId, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, string? partyId, CancellationToken cancellationToken = default);
    }
}
