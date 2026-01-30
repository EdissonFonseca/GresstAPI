using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Packaging with automatic mapping to/from Embalaje (BD)
/// </summary>
public class PackagingRepository : IRepository<Packaging>
{
    private readonly InfrastructureDbContext _context;
    private readonly PackagingMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public PackagingRepository(
        InfrastructureDbContext context, 
        PackagingMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Packaging?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id) || !long.TryParse(id, out var idLong))
            return null;
        var dbEntity = await _context.Embalajes.FindAsync(new object[] { idLong }, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Packaging>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Embalaje no tiene IdCuenta directamente, filtrar por Activo
        var dbEntities = await _context.Embalajes
            .Where(e => e.Activo)
            .Include(e => e.IdEmbalajeSuperiorNavigation) // Parent packaging
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Packaging>> FindAsync(Expression<Func<Packaging, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // Para queries más complejas, traemos todo y filtramos en memoria
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Packaging> AddAsync(Packaging entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        dbEntity.Activo = true;

        await _context.Embalajes.AddAsync(dbEntity, cancellationToken);
        
        // Update domain entity with generated ID
        entity.Id = dbEntity.IdEmbalaje.ToString();
        
        return entity;
    }

    public Task UpdateAsync(Packaging entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Embalajes.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Packaging with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        // Update audit fields
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        
        _context.Embalajes.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Packaging entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Embalajes.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Packaging with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        
        _context.Embalajes.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Packaging, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _context.Embalajes
                .Where(e => e.Activo)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

