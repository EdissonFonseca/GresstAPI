using Gresst.Domain.Common;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;

namespace Gresst.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly GreesstDbContext _context;
    protected readonly DbSet<T> _dbSet;

    private static string EncodeCursor(T entity)
    {
        var payload = entity.CreatedAt.ToString("O"); // ISO 8601, máxima precisión
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
    }

    private static DateTime DecodeCursor(string cursor)
    {
        var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
        return DateTime.Parse(raw, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }

    public Repository(GreesstDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => e.IsActive).ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<T>> FindPagedAsync(Expression<Func<T, bool>>? predicate, string? nextCursor, int limit, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<T>().AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        if (!string.IsNullOrEmpty(nextCursor))
        {
            var createdAt = DecodeCursor(nextCursor);
            query = query.Where(e => e.CreatedAt < createdAt);
        }
        query = query.OrderByDescending(e => e.CreatedAt);

        // Traer limit + 1 para saber si hay más páginas
        var items = await query.Take(limit + 1).ToListAsync(cancellationToken);

        var hasMore = items.Count > limit;
        if (hasMore) items = items.Take(limit).ToList();

        return new PagedResult<T>
        {
            Items = items,
            NextCursor = hasMore ? EncodeCursor(items.Last()) : null,
            HasMore = hasMore
        };
    }

    public async Task<(IEnumerable<T> Items, string? Next)> FindPagedAsync(Expression<Func<T, bool>>? predicate, int limit = 50, string? next = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<T>().AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        if (!string.IsNullOrEmpty(next))
        {
            var createdAt = DecodeCursor(next);
            query = query.Where(e => e.CreatedAt < createdAt);
        }

        query = query.OrderByDescending(e => e.CreatedAt);

        var items = await query.Take(limit + 1).ToListAsync(cancellationToken);

        var hasMore = items.Count > limit;
        if (hasMore) items = items.Take(limit).ToList();

        var nextCursor = hasMore ? EncodeCursor(items.Last()) : null;

        return (items, nextCursor);
    }    

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.IsActive = false;
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbSet.CountAsync(cancellationToken);
        
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }
}

