using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

public class PersonRepository : IRepository<Person>
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
        var idString = ConvertGuidToString(id);
        var dbEntity = await _context.Personas.FindAsync(new object[] { idString }, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = ConvertGuidToLong(accountId);
        
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
        dbEntity.IdCuenta = ConvertGuidToLong(_currentUserService.GetCurrentAccountId());
        
        // Generate IdPersona if empty
        if (string.IsNullOrEmpty(dbEntity.IdPersona))
        {
            dbEntity.IdPersona = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 40);
        }
        
        await _context.Personas.AddAsync(dbEntity, cancellationToken);
        
        entity.Id = ConvertStringToGuid(dbEntity.IdPersona);
        return entity;
    }

    public Task UpdateAsync(Person entity, CancellationToken cancellationToken = default)
    {
        var idString = ConvertGuidToString(entity.Id);
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
        var idString = ConvertGuidToString(entity.Id);
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
        var accountIdLong = ConvertGuidToLong(accountId);
        
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
    private Guid ConvertLongToGuid(long id)
    {
        if (id == 0) return Guid.Empty;
        return new Guid(id.ToString().PadLeft(32, '0'));
    }

    private Guid ConvertStringToGuid(string id)
    {
        if (string.IsNullOrEmpty(id)) return Guid.Empty;
        
        if (Guid.TryParse(id, out var guid))
            return guid;
        
        return new Guid(id.PadLeft(32, '0').Substring(0, 32));
    }

    private long ConvertGuidToLong(Guid guid)
    {
        if (guid == Guid.Empty) return 0;
        
        var guidString = guid.ToString().Replace("-", "");
        var numericPart = new string(guidString.Where(char.IsDigit).Take(18).ToArray());
        
        return long.TryParse(numericPart, out var result) ? result : 0;
    }

    private string ConvertGuidToString(Guid guid)
    {
        if (guid == Guid.Empty) return string.Empty;
        return guid.ToString().Replace("-", "").Substring(0, 40);
    }

    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return ConvertGuidToLong(userId);
    }
}

