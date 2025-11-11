using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

public class ManagementRepository : IRepository<Management>
{
    private readonly InfrastructureDbContext _context;
    private readonly ManagementMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public ManagementRepository(
        InfrastructureDbContext context,
        ManagementMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Management?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(id);
        var dbEntity = await _context.Gestions
            .Include(g => g.IdResiduoNavigation)
            .Include(g => g.IdResponsableNavigation)
            .Include(g => g.IdServicioNavigation)
            .FirstOrDefaultAsync(g => g.IdMovimiento == idLong, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Management>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dbEntities = await _context.Gestions
            .Include(g => g.IdResiduoNavigation)
            .Include(g => g.IdResponsableNavigation)
            .Include(g => g.IdServicioNavigation)
            .OrderByDescending(g => g.Fecha)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Management>> FindAsync(Expression<Func<Management, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Management> AddAsync(Management entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        
        await _context.Gestions.AddAsync(dbEntity, cancellationToken);
        
        entity.Id = ConvertLongToGuid(dbEntity.IdMovimiento);
        return entity;
    }

    public Task UpdateAsync(Management entity, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(entity.Id);
        var dbEntity = _context.Gestions.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Management with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Gestions.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Management entity, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(entity.Id);
        var dbEntity = _context.Gestions.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Management with ID {entity.Id} not found");

        // Hard delete
        _context.Gestions.Remove(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Management, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _context.Gestions.CountAsync(cancellationToken);
        
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

    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return ConvertGuidToLong(userId);
    }
}

