using Gresst.Domain.Entities;
using Gresst.Domain.Enums;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

public class WasteRepository : IRepository<Waste>
{
    private readonly InfrastructureDbContext _context;
    private readonly WasteMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public WasteRepository(
        InfrastructureDbContext context,
        WasteMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Waste?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(id);
        var dbEntity = await _context.Residuos
            .Include(r => r.IdMaterialNavigation)
            .Include(r => r.IdPropietarioNavigation)
            .FirstOrDefaultAsync(r => r.IdResiduo == idLong, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Waste>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dbEntities = await _context.Residuos
            .Include(r => r.IdMaterialNavigation)
            .Include(r => r.IdPropietarioNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Waste>> FindAsync(Expression<Func<Waste, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Waste> AddAsync(Waste entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        dbEntity.FechaIngreso = entity.GeneratedAt;
        
        await _context.Residuos.AddAsync(dbEntity, cancellationToken);
        
        // Return domain entity with generated ID
        entity.Id = ConvertLongToGuid(dbEntity.IdResiduo);
        return entity;
    }

    public Task UpdateAsync(Waste entity, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(entity.Id);
        var dbEntity = _context.Residuos.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Waste with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Residuos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Waste entity, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(entity.Id);
        var dbEntity = _context.Residuos.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Waste with ID {entity.Id} not found");

        // Hard delete ya que Residuo no tiene campo Activo
        _context.Residuos.Remove(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Waste, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _context.Residuos.CountAsync(cancellationToken);
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }

    // Helper methods
    private Guid ConvertLongToGuid(long id)
    {
        if (id == 0) return Guid.Empty;
        return new Guid(id.ToString().PadLeft(32, '0'));
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

