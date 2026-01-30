using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for PersonMaterial with automatic mapping to/from PersonaMaterial
/// </summary>
public class PersonMaterialRepository : IRepository<PersonMaterial>
{
    private readonly InfrastructureDbContext _context;
    private readonly PersonMaterialMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public PersonMaterialRepository(
        InfrastructureDbContext context,
        PersonMaterialMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PersonMaterial?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // PersonMaterial has composite key, so we need to find by MaterialId
        // This is a limitation - we'd need MaterialId to find it properly
        // For now, return null as we need both PersonId and MaterialId
        await Task.CompletedTask;
        return null;
    }

    public async Task<IEnumerable<PersonMaterial>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        var dbEntities = await _context.PersonaMaterials
            .Where(pm => pm.Activo && pm.IdCuenta == accountIdLong)
            .Include(pm => pm.IdMaterialNavigation)
            .Include(pm => pm.IdPersonaNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<PersonMaterial>> FindAsync(Expression<Func<PersonMaterial, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        var dbEntities = await _context.PersonaMaterials
            .Where(pm => pm.Activo && pm.IdCuenta == accountIdLong)
            .Include(pm => pm.IdMaterialNavigation)
            .Include(pm => pm.IdPersonaNavigation)
            .ToListAsync(cancellationToken);

        var domainEntities = dbEntities.Select(_mapper.ToDomain).ToList();
        return domainEntities.Where(predicate.Compile());
    }

    public async Task<PersonMaterial> AddAsync(PersonMaterial entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        dbEntity.IdCuenta = string.IsNullOrEmpty(_currentUserService.GetCurrentAccountId()) ? 0L : long.Parse(_currentUserService.GetCurrentAccountId());
        dbEntity.Activo = true;
        
        await _context.PersonaMaterials.AddAsync(dbEntity, cancellationToken);
        
        return entity;
    }

    public Task UpdateAsync(PersonMaterial entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _context.PersonaMaterials
            .FirstOrDefault(pm => pm.IdPersona == (entity.PersonId ?? string.Empty) 
                && pm.IdMaterial == IdConversion.ToLongFromString(entity.MaterialId)
                && pm.IdCuenta == IdConversion.ToLongFromString(entity.AccountId));
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"PersonMaterial not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.PersonaMaterials.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PersonMaterial entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _context.PersonaMaterials
            .FirstOrDefault(pm => pm.IdPersona == (entity.PersonId ?? string.Empty) 
                && pm.IdMaterial == IdConversion.ToLongFromString(entity.MaterialId)
                && pm.IdCuenta == IdConversion.ToLongFromString(entity.AccountId));
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"PersonMaterial not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.PersonaMaterials.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<PersonMaterial, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        if (predicate == null)
        {
            return await _context.PersonaMaterials
                .Where(pm => pm.Activo && pm.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }

    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return IdConversion.ToLongFromString(userId);
    }
}

