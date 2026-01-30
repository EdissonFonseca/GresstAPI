using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Treatment with automatic mapping to/from Tratamiento (BD)
/// </summary>
public class TreatmentRepository : IRepository<Treatment>
{
    private readonly InfrastructureDbContext _context;
    private readonly TreatmentMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public TreatmentRepository(
        InfrastructureDbContext context, 
        TreatmentMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Treatment?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id) || !long.TryParse(id, out var idLong))
            return null;
        var dbEntity = await _context.Tratamientos
            .Include(t => t.IdServicioNavigation)
            .FirstOrDefaultAsync(t => t.IdTratamiento == idLong, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Treatment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Tratamiento puede ser público o privado, filtrar por Activo
        var dbEntities = await _context.Tratamientos
            .Where(t => t.Activo)
            .Include(t => t.IdServicioNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Treatment>> FindAsync(Expression<Func<Treatment, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // Para queries más complejas, traemos todo y filtramos en memoria
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Treatment> AddAsync(Treatment entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        dbEntity.Activo = true;

        await _context.Tratamientos.AddAsync(dbEntity, cancellationToken);
        
        // Update domain entity with generated ID
        entity.Id = dbEntity.IdTratamiento.ToString();
        
        return entity;
    }

    public Task UpdateAsync(Treatment entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Tratamientos.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Treatment with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        // Update audit fields
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        
        _context.Tratamientos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Treatment entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Tratamientos.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Treatment with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        
        _context.Tratamientos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Treatment, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _context.Tratamientos
                .Where(t => t.Activo)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

