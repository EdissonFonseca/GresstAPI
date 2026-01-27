using Gresst.Domain.Common;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Generic repository for entities that don't have specific mappers yet
/// This is a temporary solution until all mappers are created
/// </summary>
public class GenericInfraRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly InfrastructureDbContext _context;

    public GenericInfraRepository(InfrastructureDbContext context)
    {
        _context = context;
    }

    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // Temporary implementation - returns null
        // Real implementation needs mapper
        await Task.CompletedTask;
        return null;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return new List<T>();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return new List<T>();
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return entity;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return 0;
    }
}

