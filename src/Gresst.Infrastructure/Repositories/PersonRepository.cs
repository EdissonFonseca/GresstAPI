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

    public async Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var idString = GuidStringConverter.ToString(id);
        var dbEntity = await _context.Personas.FindAsync(new object[] { idString }, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
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
        dbEntity.IdCuenta = GuidLongConverter.ToLong(_currentUserService.GetCurrentAccountId());
        
        // Generate IdPersona if empty
        if (string.IsNullOrEmpty(dbEntity.IdPersona))
        {
            dbEntity.IdPersona = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 40);
        }
        
        await _context.Personas.AddAsync(dbEntity, cancellationToken);
        
        entity.Id = GuidStringConverter.ToGuid(dbEntity.IdPersona);
        return entity;
    }

    public Task UpdateAsync(Person entity, CancellationToken cancellationToken = default)
    {
        var idString = GuidStringConverter.ToString(entity.Id);
        var dbEntity = _context.Personas.Find(idString);
        
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
        var idString = GuidStringConverter.ToString(entity.Id);
        var dbEntity = _context.Personas.Find(idString);
        
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
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
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
        return GuidLongConverter.ToLong(userId);
    }

    // IPersonRepository methods
    public async Task<IEnumerable<Person>> GetByRoleAsync(string roleCode, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
        var dbEntities = await _context.Personas
            .Where(p => p.Activo && 
                       p.IdCuenta == accountIdLong && 
                       p.IdRol == roleCode)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<Person?> GetByIdAndRoleAsync(Guid id, string roleCode, CancellationToken cancellationToken = default)
    {
        var idString = GuidStringConverter.ToString(id);
        var dbEntity = await _context.Personas.FindAsync(new object[] { idString }, cancellationToken);
        
        if (dbEntity == null || dbEntity.IdRol != roleCode)
            return null;
        
        return _mapper.ToDomain(dbEntity);
    }

    public async Task SetPersonRoleAsync(Guid personId, string roleCode, CancellationToken cancellationToken = default)
    {
        var idString = GuidStringConverter.ToString(personId);
        var dbEntity = await _context.Personas.FindAsync(new object[] { idString }, cancellationToken);
        
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
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        var personId = _currentUserService.GetCurrentAccountPersonId();
        var personIdString = GuidStringConverter.ToString(personId);

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

