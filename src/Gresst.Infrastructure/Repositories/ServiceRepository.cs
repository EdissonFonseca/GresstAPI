using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Service with automatic mapping to/from Servicio (BD)
/// </summary>
public class ServiceRepository : IRepository<Service>
{
    private readonly InfrastructureDbContext _context;
    private readonly ServiceMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public ServiceRepository(
        InfrastructureDbContext context, 
        ServiceMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Service?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id) || !long.TryParse(id, out var idLong))
            return null;
        var dbEntity = await _context.Servicios.FindAsync(new object[] { idLong }, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Service>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Servicio no tiene IdCuenta directamente, filtrar por Activo
        var dbEntities = await _context.Servicios
            .Where(s => s.Activo)
            .Include(s => s.IdCategoriaNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Service>> FindAsync(Expression<Func<Service, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // Para queries más complejas, traemos todo y filtramos en memoria
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Service> AddAsync(Service entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        dbEntity.Activo = true;

        await _context.Servicios.AddAsync(dbEntity, cancellationToken);
        
        // Update domain entity with generated ID
        entity.Id = dbEntity.IdServicio.ToString();
        
        return entity;
    }

    public Task UpdateAsync(Service entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Servicios.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Service with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        // Update audit fields
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.Servicios.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Service entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Servicios.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Service with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.Servicios.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Service, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _context.Servicios
                .Where(s => s.Activo)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

