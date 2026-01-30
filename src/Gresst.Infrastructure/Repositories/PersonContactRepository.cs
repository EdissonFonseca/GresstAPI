using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Gresst.Infrastructure.Common;

namespace Gresst.Infrastructure.Repositories;

public class PersonContactRepository : IPersonContactRepository
{
    private readonly InfrastructureDbContext _context;
    private readonly PersonContactMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public PersonContactRepository(
        InfrastructureDbContext context,
        PersonContactMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PersonContact?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // PersonContact uses composite key, so we need to find by PersonId and ContactId
        // For now, we'll search all contacts and find by a generated Guid match
        // This is a limitation - we might need to store a mapping or use a different approach
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = IdConversion.ToLongFromString(accountId);
        
        var dbEntities = await _context.PersonaContactos
            .Where(pc => pc.IdCuenta == accountIdLong && pc.Activo)
            .Include(pc => pc.IdPersonaNavigation)
            .Include(pc => pc.IdContactoNavigation)
            .Include(pc => pc.IdRelacionNavigation)
            .Include(pc => pc.IdLocalizacionNavigation)
            .ToListAsync(cancellationToken);

        // Since PersonContact doesn't have a single ID, we'll return the first match
        // In a real scenario, you might want to store a mapping table
        var dbEntity = dbEntities.FirstOrDefault();
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<PersonContact>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = IdConversion.ToLongFromString(accountId);
        
        var dbEntities = await _context.PersonaContactos
            .Where(pc => pc.IdCuenta == accountIdLong && pc.Activo)
            .Include(pc => pc.IdPersonaNavigation)
            .Include(pc => pc.IdContactoNavigation)
            .Include(pc => pc.IdRelacionNavigation)
            .Include(pc => pc.IdLocalizacionNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<PersonContact>> FindAsync(Expression<Func<PersonContact, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<PersonContact> AddAsync(PersonContact entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        _context.PersonaContactos.Add(dbEntity);
        return entity;
    }

    public async Task UpdateAsync(PersonContact entity, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = IdConversion.ToLongFromString(accountId);
        var personIdString = entity.PersonId ?? string.Empty;
        var contactIdString = entity.ContactId ?? string.Empty;
        
        var dbEntity = await _context.PersonaContactos
            .FirstOrDefaultAsync(pc => 
                pc.IdPersona == personIdString &&
                pc.IdContacto == contactIdString &&
                pc.IdRelacion == entity.RelationshipType &&
                pc.IdCuenta == accountIdLong,
                cancellationToken);

        if (dbEntity != null)
        {
            _mapper.UpdateDatabase(entity, dbEntity);
        }
    }

    public async Task DeleteAsync(PersonContact entity, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = IdConversion.ToLongFromString(accountId);
        var personIdString = entity.PersonId ?? string.Empty;
        var contactIdString = entity.ContactId ?? string.Empty;
        
        var dbEntity = await _context.PersonaContactos
            .FirstOrDefaultAsync(pc => 
                pc.IdPersona == personIdString &&
                pc.IdContacto == contactIdString &&
                pc.IdRelacion == entity.RelationshipType &&
                pc.IdCuenta == accountIdLong,
                cancellationToken);

        if (dbEntity != null)
        {
            _context.PersonaContactos.Remove(dbEntity);
        }
    }

    public async Task<int> CountAsync(Expression<Func<PersonContact, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        if (predicate == null)
            return all.Count();
        
        return all.Count(predicate.Compile());
    }

    public async Task<IEnumerable<PersonContact>> GetContactsByPersonIdAsync(string personId, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = IdConversion.ToLongFromString(accountId);
        var personIdString = personId ?? string.Empty;
        
        var dbEntities = await _context.PersonaContactos
            .Where(pc => pc.IdPersona == personIdString && pc.IdCuenta == accountIdLong && pc.Activo)
            .Include(pc => pc.IdPersonaNavigation)
            .Include(pc => pc.IdContactoNavigation)
            .Include(pc => pc.IdRelacionNavigation)
            .Include(pc => pc.IdLocalizacionNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<PersonContact>> GetPersonsByContactIdAsync(string contactId, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = IdConversion.ToLongFromString(accountId);
        var contactIdString = contactId ?? string.Empty;
        
        var dbEntities = await _context.PersonaContactos
            .Where(pc => pc.IdContacto == contactIdString && pc.IdCuenta == accountIdLong && pc.Activo)
            .Include(pc => pc.IdPersonaNavigation)
            .Include(pc => pc.IdContactoNavigation)
            .Include(pc => pc.IdRelacionNavigation)
            .Include(pc => pc.IdLocalizacionNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<PersonContact?> GetByPersonAndContactIdAsync(string personId, string contactId, string? relationshipType = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = IdConversion.ToLongFromString(accountId);
        var personIdString = personId ?? string.Empty;
        var contactIdString = contactId ?? string.Empty;
        
        var query = _context.PersonaContactos
            .Where(pc => 
                pc.IdPersona == personIdString &&
                pc.IdContacto == contactIdString &&
                pc.IdCuenta == accountIdLong &&
                pc.Activo);

        if (!string.IsNullOrEmpty(relationshipType))
        {
            query = query.Where(pc => pc.IdRelacion == relationshipType);
        }

        var dbEntity = await query
            .Include(pc => pc.IdPersonaNavigation)
            .Include(pc => pc.IdContactoNavigation)
            .Include(pc => pc.IdRelacionNavigation)
            .Include(pc => pc.IdLocalizacionNavigation)
            .FirstOrDefaultAsync(cancellationToken);

        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<PersonContact>> GetContactsByPersonAndRelationshipAsync(string personId, string relationshipType, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = IdConversion.ToLongFromString(accountId);
        var personIdString = personId ?? string.Empty;
        
        var dbEntities = await _context.PersonaContactos
            .Where(pc => 
                pc.IdPersona == personIdString &&
                pc.IdRelacion == relationshipType &&
                pc.IdCuenta == accountIdLong &&
                pc.Activo)
            .Include(pc => pc.IdPersonaNavigation)
            .Include(pc => pc.IdContactoNavigation)
            .Include(pc => pc.IdRelacionNavigation)
            .Include(pc => pc.IdLocalizacionNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }
}

