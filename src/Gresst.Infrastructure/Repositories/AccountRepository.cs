using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Account with automatic mapping to/from Cuentum
/// Follows the same pattern as FacilityRepository
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly InfrastructureDbContext _context;
    private readonly AccountMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

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
        var idLong = IdConversion.ToLongFromString(id);
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

    public async Task<Account> AddAsync(Account entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        
        await _context.Cuenta.AddAsync(dbEntity, cancellationToken);
        
        entity.Id = IdConversion.ToStringFromLong(dbEntity.IdCuenta);
        return entity;
    }

    public Task UpdateAsync(Account entity, CancellationToken cancellationToken = default)
    {
        var idLong = IdConversion.ToLongFromString(entity.Id);
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
        var idLong = IdConversion.ToLongFromString(entity.Id);
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

    // Métodos específicos de IAccountRepository
    public async Task<Account?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        
        var dbEntity = await _context.Cuenta
            .Include(c => c.IdPersonaNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .FirstOrDefaultAsync(c => c.IdUsuario == userIdLong, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<Account?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        // Si tienes un campo código en Cuentum
        var dbEntity = await _context.Cuenta
            .Include(c => c.IdPersonaNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .FirstOrDefaultAsync(c => c.Nombre == code, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return IdConversion.ToLongFromString(userId);
    }
}

