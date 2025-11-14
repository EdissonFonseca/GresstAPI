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
public class FacilityRepository : IRepository<Facility>
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

    public async Task<Facility?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(id);
        var dbEntity = await _context.Depositos
            .Include(d => d.IdPersonaNavigation)
            .Include(d => d.IdSuperiorNavigation) // Parent facility
            .FirstOrDefaultAsync(d => d.IdDeposito == idLong, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Facility>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = ConvertGuidToLong(accountId);
        
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
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        dbEntity.IdCuenta = ConvertGuidToLong(_currentUserService.GetCurrentAccountId());
        
        await _context.Depositos.AddAsync(dbEntity, cancellationToken);
        
        // Return domain entity with generated ID
        entity.Id = new Guid(dbEntity.IdDeposito.ToString().PadLeft(32, '0'));
        return entity;
    }

    public Task UpdateAsync(Facility entity, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(entity.Id);
        var dbEntity = _context.Depositos.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Facility with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        // Update audit fields
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Depositos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Facility entity, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(entity.Id);
        var dbEntity = _context.Depositos.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Facility with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Depositos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Facility, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = ConvertGuidToLong(accountId);
        
        if (predicate == null)
        {
            return await _context.Depositos
                .Where(d => d.Activo && d.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }

    // Helper methods para conversión de tipos
    private long ConvertGuidToLong(Guid guid)
    {
        if (guid == Guid.Empty) return 0;
        
        var guidString = guid.ToString().Replace("-", "");
        var numericPart = new string(guidString.Where(char.IsDigit).Take(18).ToArray());
        
        return long.TryParse(numericPart, out var result) ? result : 0;
    }

    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return ConvertGuidToLong(userId);
    }
}

