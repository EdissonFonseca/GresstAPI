using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Supply with automatic mapping to/from Insumo (BD)
/// </summary>
public class SupplyRepository : IRepository<Supply>
{
    private readonly InfrastructureDbContext _context;
    private readonly SupplyMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public SupplyRepository(
        InfrastructureDbContext context, 
        SupplyMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Supply?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var idLong = GuidLongConverter.ToLong(id);
        var dbEntity = await _context.Insumos.FindAsync(new object[] { idLong }, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Supply>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Insumo puede ser público o privado, filtrar por Activo
        // Los insumos públicos son visibles para todos, los privados solo para la cuenta que los creó
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
        var dbEntities = await _context.Insumos
            .Where(i => i.Activo && (i.Publico || i.IdUsuarioCreacion == accountIdLong))
            .Include(i => i.IdInsumoSuperiorNavigation) // Parent supply
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Supply>> FindAsync(Expression<Func<Supply, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // Para queries más complejas, traemos todo y filtramos en memoria
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Supply> AddAsync(Supply entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        dbEntity.Activo = true;

        await _context.Insumos.AddAsync(dbEntity, cancellationToken);
        
        // Update domain entity with generated ID
        entity.Id = GuidLongConverter.ToGuid(dbEntity.IdInsumo);
        
        return entity;
    }

    public Task UpdateAsync(Supply entity, CancellationToken cancellationToken = default)
    {
        var idLong = GuidLongConverter.ToLong(entity.Id);
        var dbEntity = _context.Insumos.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Supply with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        // Update audit fields
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.Insumos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Supply entity, CancellationToken cancellationToken = default)
    {
        var idLong = GuidLongConverter.ToLong(entity.Id);
        var dbEntity = _context.Insumos.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Supply with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.Insumos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Supply, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
        if (predicate == null)
        {
            return await _context.Insumos
                .Where(i => i.Activo && (i.Publico || i.IdUsuarioCreacion == accountIdLong))
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

