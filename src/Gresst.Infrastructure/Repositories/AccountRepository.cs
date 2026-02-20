using Gresst.Domain.Common;
using Gresst.Domain.Identity;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Account with automatic mapping to/from Cuentum
/// Follows the same pattern as FacilityRepository
/// </summary>
public class AccountRepository : IRepository<Account>
{
    private readonly InfrastructureDbContext _context;
    private readonly AccountMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    private static string EncodeCreatedAtCursor(DateTime createdAt)
    {
        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(createdAt.ToString("O")));
    }

    private static DateTime DecodeCreatedAtCursor(string cursor)
    {
        var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
        return DateTime.Parse(raw, null, DateTimeStyles.RoundtripKind);
    }

    public AccountRepository(
        InfrastructureDbContext context,
        AccountMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    // Métodos de IRepository<Account>
    public async Task<Account?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        var idLong = long.TryParse(id, out var value) ? value: 0;
        if (idLong == 0)
            return null;
        var dbEntity = await _context.Cuenta
            .Include(c => c.IdPersonaNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .FirstOrDefaultAsync(c => c.IdCuenta == idLong, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dbEntities = await _context.Cuenta
            .Include(c => c.IdPersonaNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .Where(c => c.IdEstado != "I") // No incluir inactivos
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Account>> FindAsync(Expression<Func<Account, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<(IEnumerable<Account> Items, string? Next)> FindPagedAsync(Expression<Func<Account, bool>>? predicate, int limit = 50, string? next = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Cuenta
            .Include(c => c.IdPersonaNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .Where(c => c.IdEstado != "I");

        if (!string.IsNullOrEmpty(next))
        {
            var createdAt = DecodeCreatedAtCursor(next);
            query = query.Where(e => e.FechaCreacion < createdAt);
        }

        query = query.OrderByDescending(e => e.FechaCreacion);

        var dbEntities = await query.Take(limit + 1).ToListAsync(cancellationToken);

        var hasMore = dbEntities.Count > limit;
        if (hasMore) dbEntities = dbEntities.Take(limit).ToList();

        var nextCursor = hasMore ? EncodeCreatedAtCursor(dbEntities.Last().FechaCreacion) : null;

        return (dbEntities.Select(_mapper.ToDomain).ToList(), nextCursor);
    }
    public async Task<Account> AddAsync(Account entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        
        await _context.Cuenta.AddAsync(dbEntity, cancellationToken);
        
        entity.Id = dbEntity.IdCuenta.ToString();
        return entity;
    }

    public Task UpdateAsync(Account entity, CancellationToken cancellationToken = default)
    {
        var idLong = long.TryParse(entity.Id, out var value) ? value: 0;
        var dbEntity = _context.Cuenta.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Account with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Cuenta.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Account entity, CancellationToken cancellationToken = default)
    {
        var idLong = long.TryParse(entity.Id, out var value) ? value: 0;
        var dbEntity = _context.Cuenta.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Account with ID {entity.Id} not found");

        // Soft delete
        dbEntity.IdEstado = "I"; // Inactivo
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Cuenta.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Account, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _context.Cuenta
                .Where(c => c.IdEstado != "I")
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }

    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return long.TryParse(userId, out var value) ? value : 0;
    }
}

