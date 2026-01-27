using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for PersonService with automatic mapping to/from PersonaServicio (BD)
/// </summary>
public class PersonServiceRepository : IRepository<PersonService>
{
    private readonly InfrastructureDbContext _context;
    private readonly PersonServiceMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public PersonServiceRepository(
        InfrastructureDbContext context,
        PersonServiceMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PersonService?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // PersonService has composite key, so GetByIdAsync needs to be adapted
        // This generic GetById will not work directly for composite keys without more info
        await Task.CompletedTask;
        return null;
    }

    public async Task<IEnumerable<PersonService>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);

        var dbEntities = await _context.PersonaServicios
            .Where(ps => ps.Activo && ps.IdCuenta == accountIdLong)
            .Include(ps => ps.IdPersonaNavigation)
            .Include(ps => ps.IdServicioNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<PersonService>> FindAsync(Expression<Func<PersonService, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<PersonService> AddAsync(PersonService entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        dbEntity.IdCuenta = string.IsNullOrEmpty(_currentUserService.GetCurrentAccountId()) ? 0L : long.Parse(_currentUserService.GetCurrentAccountId());
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        dbEntity.Activo = true;

        await _context.PersonaServicios.AddAsync(dbEntity, cancellationToken);
        
        // The domain entity's ID is a generated Guid, not directly from DB composite key
        return entity;
    }

    public Task UpdateAsync(PersonService entity, CancellationToken cancellationToken = default)
    {
        var accountIdLong = string.IsNullOrEmpty(_currentUserService.GetCurrentAccountId()) ? 0L : long.Parse(_currentUserService.GetCurrentAccountId());
        var personIdString = GuidStringConverter.ToString(entity.PersonId);
        var serviceIdLong = GuidLongConverter.ToLong(entity.ServiceId);
        var startDate = DateOnly.FromDateTime(entity.StartDate);

        var dbEntity = _context.PersonaServicios.Find(personIdString, serviceIdLong, startDate, accountIdLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"PersonService relationship for Person {entity.PersonId}, Service {entity.ServiceId} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.PersonaServicios.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PersonService entity, CancellationToken cancellationToken = default)
    {
        var accountIdLong = string.IsNullOrEmpty(_currentUserService.GetCurrentAccountId()) ? 0L : long.Parse(_currentUserService.GetCurrentAccountId());
        var personIdString = GuidStringConverter.ToString(entity.PersonId);
        var serviceIdLong = GuidLongConverter.ToLong(entity.ServiceId);
        var startDate = DateOnly.FromDateTime(entity.StartDate);

        var dbEntity = _context.PersonaServicios.Find(personIdString, serviceIdLong, startDate, accountIdLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"PersonService relationship for Person {entity.PersonId}, Service {entity.ServiceId} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.PersonaServicios.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<PersonService, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        if (predicate == null)
        {
            return await _context.PersonaServicios
                .Where(ps => ps.Activo && ps.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

