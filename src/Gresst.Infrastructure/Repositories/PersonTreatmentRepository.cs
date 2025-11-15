using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for PersonTreatment with automatic mapping to/from PersonaTratamiento (BD)
/// </summary>
public class PersonTreatmentRepository : IRepository<PersonTreatment>
{
    private readonly InfrastructureDbContext _context;
    private readonly PersonTreatmentMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public PersonTreatmentRepository(
        InfrastructureDbContext context,
        PersonTreatmentMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PersonTreatment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // PersonTreatment has composite key, so GetByIdAsync needs to be adapted
        await Task.CompletedTask;
        return null;
    }

    public async Task<IEnumerable<PersonTreatment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = GuidLongConverter.ToLong(accountId);

        var dbEntities = await _context.PersonaTratamientos
            .Where(pt => pt.Activo && pt.IdCuenta == accountIdLong)
            .Include(pt => pt.IdPersonaNavigation)
            .Include(pt => pt.IdTratamientoNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<PersonTreatment>> FindAsync(Expression<Func<PersonTreatment, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<PersonTreatment> AddAsync(PersonTreatment entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        dbEntity.IdCuenta = GuidLongConverter.ToLong(_currentUserService.GetCurrentAccountId());
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        dbEntity.Activo = true;

        await _context.PersonaTratamientos.AddAsync(dbEntity, cancellationToken);
        
        return entity;
    }

    public Task UpdateAsync(PersonTreatment entity, CancellationToken cancellationToken = default)
    {
        var accountIdLong = GuidLongConverter.ToLong(_currentUserService.GetCurrentAccountId());
        var personIdString = GuidStringConverter.ToString(entity.PersonId);
        var treatmentIdLong = GuidLongConverter.ToLong(entity.TreatmentId);

        var dbEntity = _context.PersonaTratamientos.Find(personIdString, treatmentIdLong, accountIdLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"PersonTreatment relationship for Person {entity.PersonId}, Treatment {entity.TreatmentId} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.PersonaTratamientos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(PersonTreatment entity, CancellationToken cancellationToken = default)
    {
        var accountIdLong = GuidLongConverter.ToLong(_currentUserService.GetCurrentAccountId());
        var personIdString = GuidStringConverter.ToString(entity.PersonId);
        var treatmentIdLong = GuidLongConverter.ToLong(entity.TreatmentId);

        var dbEntity = _context.PersonaTratamientos.Find(personIdString, treatmentIdLong, accountIdLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"PersonTreatment relationship for Person {entity.PersonId}, Treatment {entity.TreatmentId} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.PersonaTratamientos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<PersonTreatment, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
        if (predicate == null)
        {
            return await _context.PersonaTratamientos
                .Where(pt => pt.Activo && pt.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

