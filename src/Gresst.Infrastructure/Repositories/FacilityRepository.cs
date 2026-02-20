using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Facility with automatic mapping to/from Deposito
/// </summary>
public class FacilityRepository : IPartyRepository<Facility>
{
    private readonly InfrastructureDbContext _context;
    private readonly FacilityMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public FacilityRepository(
        InfrastructureDbContext context, 
        FacilityMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Facility?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id) || !long.TryParse(id, out var idLong))
            return null;
        var dbEntity = await _context.Depositos
            .Include(d => d.IdPersonaNavigation)
            .Include(d => d.IdSuperiorNavigation) // Parent facility
            .FirstOrDefaultAsync(d => d.IdDeposito == idLong, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Facility>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        var dbEntities = await _context.Depositos
            .Where(d => d.Activo && d.IdCuenta == accountIdLong)
            .Include(d => d.IdPersonaNavigation)
            .Include(d => d.IdSuperiorNavigation) // Parent facility
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Facility>> FindAsync(Expression<Func<Facility, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // Para queries más complejas, necesitarías traducir el Expression
        // Por ahora, traemos todo y filtramos en memoria (menos eficiente pero funcional)
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Facility> AddAsync(Facility entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = string.IsNullOrEmpty(userId) ? 0L : long.Parse(userId);

        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = userIdLong;
        dbEntity.IdCuenta = string.IsNullOrEmpty(_currentUserService.GetCurrentAccountId()) ? null : long.Parse(_currentUserService.GetCurrentAccountId());
        
        await _context.Depositos.AddAsync(dbEntity, cancellationToken);
        
        // Return domain entity with generated ID
        entity.Id = dbEntity.IdDeposito.ToString();
        return entity;
    }

    public Task UpdateAsync(Facility entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Depositos.Find(idLong);
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = string.IsNullOrEmpty(userId) ? 0L : long.Parse(userId);

        if (dbEntity == null)
            throw new KeyNotFoundException($"Facility with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        // Update audit fields
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = userIdLong;
        
        _context.Depositos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Facility entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Depositos.Find(idLong);
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = string.IsNullOrEmpty(userId) ? 0L : long.Parse(userId);

        if (dbEntity == null)
            throw new KeyNotFoundException($"Facility with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = userIdLong;
        
        _context.Depositos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Facility, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        if (predicate == null)
        {
            return await _context.Depositos
                .Where(d => d.Activo && d.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }

    Task<IEnumerable<Facility>> IPartyRepository<Facility>.GetAllAsync(string? partyId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<Facility>> IPartyRepository<Facility>.FindAsync(Expression<Func<Facility, bool>> predicate, string? partyId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<(IEnumerable<Facility> Items, string? Next)> IPartyRepository<Facility>.FindPagedAsync(Expression<Func<Facility, bool>> predicate, string? partyId, int limit, string? next, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<int> IPartyRepository<Facility>.CountAsync(Expression<Func<Facility, bool>>? predicate, string? partyId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<Facility> IPartyRepository<Facility>.AddAsync(Facility entity, string? partyId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task IPartyRepository<Facility>.UpdateAsync(Facility entity, string? partyId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task IPartyRepository<Facility>.DeleteAsync(Facility entity, string? partyId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

