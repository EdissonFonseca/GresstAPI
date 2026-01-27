using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for WasteClass with automatic mapping to/from TipoResiduo (BD)
/// </summary>
public class WasteClassRepository : IRepository<WasteClass>
{
    private readonly InfrastructureDbContext _context;
    private readonly WasteClassMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public WasteClassRepository(
        InfrastructureDbContext context, 
        WasteClassMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<WasteClass?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id) || !int.TryParse(id, out var idInt))
            return null;
        var dbEntity = await _context.TipoResiduos.FindAsync(new object[] { idInt }, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<WasteClass>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // TipoResiduo puede ser público o privado, filtrar por Activo
        var dbEntities = await _context.TipoResiduos
            .Where(tr => tr.Activo)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<WasteClass>> FindAsync(Expression<Func<WasteClass, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // Para queries más complejas, traemos todo y filtramos en memoria
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<WasteClass> AddAsync(WasteClass entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        dbEntity.Activo = true;

        await _context.TipoResiduos.AddAsync(dbEntity, cancellationToken);
        
        // Update domain entity with generated ID
        entity.Id = dbEntity.IdTipoResiduo.ToString();
        
        return entity;
    }

    public Task UpdateAsync(WasteClass entity, CancellationToken cancellationToken = default)
    {
        var idInt = string.IsNullOrEmpty(entity.Id) ? 0 : int.Parse(entity.Id);
        var dbEntity = _context.TipoResiduos.Find(idInt);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"WasteClass with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        // Update audit fields
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.TipoResiduos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(WasteClass entity, CancellationToken cancellationToken = default)
    {
        var idInt = string.IsNullOrEmpty(entity.Id) ? 0 : int.Parse(entity.Id);
        var dbEntity = _context.TipoResiduos.Find(idInt);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"WasteClass with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.TipoResiduos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<WasteClass, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _context.TipoResiduos
                .Where(tr => tr.Activo)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

