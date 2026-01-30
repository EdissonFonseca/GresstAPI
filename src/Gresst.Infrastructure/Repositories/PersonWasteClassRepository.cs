using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for PersonWasteClass with automatic mapping to/from PersonaTipoResiduo (BD)
/// </summary>
public class PersonWasteClassRepository : IRepository<PersonWasteClass>
{
    private readonly InfrastructureDbContext _context;
    private readonly PersonWasteClassMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public PersonWasteClassRepository(
        InfrastructureDbContext context,
        PersonWasteClassMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PersonWasteClass?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // PersonWasteClass has composite key, so GetByIdAsync needs to be adapted
        await Task.CompletedTask;
        return null;
    }

    public async Task<IEnumerable<PersonWasteClass>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);

        var dbEntities = await _context.PersonaTipoResiduos
            .Where(pt => pt.IdCuenta == accountIdLong)
            .Include(pt => pt.IdPersonaNavigation)
            .Include(pt => pt.IdTipoResiduoNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<PersonWasteClass>> FindAsync(Expression<Func<PersonWasteClass, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<PersonWasteClass> AddAsync(PersonWasteClass entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        dbEntity.IdCuenta = string.IsNullOrEmpty(_currentUserService.GetCurrentAccountId()) ? 0L : long.Parse(_currentUserService.GetCurrentAccountId());
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());

        await _context.PersonaTipoResiduos.AddAsync(dbEntity, cancellationToken);
        
        return entity;
    }

    public Task UpdateAsync(PersonWasteClass entity, CancellationToken cancellationToken = default)
    {
        var accountIdLong = string.IsNullOrEmpty(_currentUserService.GetCurrentAccountId()) ? 0L : long.Parse(_currentUserService.GetCurrentAccountId());
        var personIdString = entity.PersonId ?? string.Empty;
        var wasteClassIdInt = (int)IdConversion.ToLongFromString(entity.WasteClassId);

        var dbEntity = _context.PersonaTipoResiduos
            .FirstOrDefault(pt => pt.IdPersona == personIdString
                && pt.IdTipoResiduo == wasteClassIdInt
                && pt.IdCuenta == accountIdLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"PersonWasteClass relationship for Person {entity.PersonId}, WasteClass {entity.WasteClassId} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        
        _context.PersonaTipoResiduos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PersonWasteClass entity, CancellationToken cancellationToken = default)
    {
        var accountIdLong = string.IsNullOrEmpty(_currentUserService.GetCurrentAccountId()) ? 0L : long.Parse(_currentUserService.GetCurrentAccountId());
        var personIdString = entity.PersonId ?? string.Empty;
        var wasteClassIdInt = (int)IdConversion.ToLongFromString(entity.WasteClassId);

        var dbEntity = _context.PersonaTipoResiduos
            .FirstOrDefault(pt => pt.IdPersona == personIdString
                && pt.IdTipoResiduo == wasteClassIdInt
                && pt.IdCuenta == accountIdLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"PersonWasteClass relationship for Person {entity.PersonId}, WasteClass {entity.WasteClassId} not found");

        // Hard delete (no Activo field in PersonaTipoResiduo)
        _context.PersonaTipoResiduos.Remove(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<PersonWasteClass, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        if (predicate == null)
        {
            return await _context.PersonaTipoResiduos
                .Where(pt => pt.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

