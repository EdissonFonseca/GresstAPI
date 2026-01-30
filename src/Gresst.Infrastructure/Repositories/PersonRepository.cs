using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly InfrastructureDbContext _context;
    private readonly PersonMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public PersonRepository(
        InfrastructureDbContext context,
        PersonMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Person?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _context.Personas.FindAsync(new object[] { id }, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? (long?)null : long.Parse(accountId);
        
        var dbEntities = await _context.Personas
            .Where(p => p.Activo && p.IdCuenta == accountIdLong)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Person>> FindAsync(Expression<Func<Person, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Person> AddAsync(Person entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        var accountId = _currentUserService.GetCurrentAccountId();
        dbEntity.IdCuenta = string.IsNullOrEmpty(accountId) ? null : long.Parse(accountId);
        
        // Generate IdPersona if empty
        if (string.IsNullOrEmpty(dbEntity.IdPersona))
        {
            dbEntity.IdPersona = Guid.NewGuid().ToString("N")[..40];
        }
        
        await _context.Personas.AddAsync(dbEntity, cancellationToken);
        
        entity.Id = dbEntity.IdPersona;
        return entity;
    }

    public Task UpdateAsync(Person entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _context.Personas.Find(entity.Id);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Person with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Personas.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Person entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _context.Personas.Find(entity.Id);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Person with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Personas.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Person, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? (long?)null : long.Parse(accountId);
        
        if (predicate == null)
        {
            return await _context.Personas
                .Where(p => p.Activo && p.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }

    // Helper methods


    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return IdConversion.ToLongFromString(userId);
    }

    // IPersonRepository methods
    public async Task<IEnumerable<Person>> GetByRoleAsync(string roleCode, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? (long?)null : long.Parse(accountId);
        
        var dbEntities = await _context.Personas
            .Where(p => p.Activo && 
                       p.IdCuenta == accountIdLong && 
                       p.IdRol == roleCode)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<Person?> GetByIdAndRoleAsync(string id, string roleCode, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _context.Personas.FindAsync(new object[] { id }, cancellationToken);
        
        if (dbEntity == null || dbEntity.IdRol != roleCode)
            return null;
        
        return _mapper.ToDomain(dbEntity);
    }

    public async Task SetPersonRoleAsync(string personId, string roleCode, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _context.Personas.FindAsync(new object[] { personId }, cancellationToken);
        
        if (dbEntity != null)
        {
            dbEntity.IdRol = roleCode;
            dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
            dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
            _context.Personas.Update(dbEntity);
        }
    }

    public async Task<IEnumerable<Person>> GetClientsAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? (long?)null : long.Parse(accountId);
        var personId = _currentUserService.GetCurrentAccountPersonId();
        var personIdString = personId ?? string.Empty;

        var persons = await _context.PersonaContactos
            .AsNoTracking()
            .Where(pc =>
                pc.Activo &&
                pc.IdCuenta == accountIdLong &&
                pc.IdRelacion == Relation.CLIENT &&
                pc.IdPersona == personIdString
            )
            .Join(
                _context.Personas,
                pc => pc.IdContacto,
                p => p.IdPersona,
                (pc, p) => p
            )
            .ToListAsync(cancellationToken);

        return persons
            .Select(p => _mapper.ToDomain(p))
            .ToList();
    }

}

