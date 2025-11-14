using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for FacilityMaterial with automatic mapping to/from PersonaMaterialDeposito
/// </summary>
public class FacilityMaterialRepository : IRepository<FacilityMaterial>
{
    private readonly InfrastructureDbContext _context;
    private readonly FacilityMaterialMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public FacilityMaterialRepository(
        InfrastructureDbContext context,
        FacilityMaterialMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<FacilityMaterial?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // FacilityMaterial has composite key, so we need to find by MaterialId and FacilityId
        // This is a limitation - we'd need both to find it properly
        // For now, return null as we need PersonId, MaterialId, and FacilityId
        await Task.CompletedTask;
        return null;
    }

    public async Task<IEnumerable<FacilityMaterial>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
        var dbEntities = await _context.PersonaMaterialDepositos
            .Where(pmd => pmd.Activo && pmd.IdCuenta == accountIdLong)
            .Include(pmd => pmd.IdMaterialNavigation)
            .Include(pmd => pmd.IdPersonaNavigation)
            .Include(pmd => pmd.IdDepositoNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<FacilityMaterial>> FindAsync(Expression<Func<FacilityMaterial, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
        var dbEntities = await _context.PersonaMaterialDepositos
            .Where(pmd => pmd.Activo && pmd.IdCuenta == accountIdLong)
            .Include(pmd => pmd.IdMaterialNavigation)
            .Include(pmd => pmd.IdPersonaNavigation)
            .Include(pmd => pmd.IdDepositoNavigation)
            .ToListAsync(cancellationToken);

        var domainEntities = dbEntities.Select(_mapper.ToDomain).ToList();
        return domainEntities.Where(predicate.Compile());
    }

    public async Task<FacilityMaterial> AddAsync(FacilityMaterial entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        dbEntity.IdCuenta = GuidLongConverter.ToLong(entity.AccountId);
        dbEntity.Activo = entity.IsHandled;
        
        await _context.PersonaMaterialDepositos.AddAsync(dbEntity, cancellationToken);
        
        return entity;
    }

    public Task UpdateAsync(FacilityMaterial entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _context.PersonaMaterialDepositos
            .FirstOrDefault(pmd => pmd.IdPersona == GuidLongConverter.GuidToString(entity.PersonId) 
                && pmd.IdMaterial == GuidLongConverter.ToLong(entity.MaterialId)
                && pmd.IdDeposito == GuidLongConverter.ToLong(entity.FacilityId)
                && pmd.IdCuenta == GuidLongConverter.ToLong(entity.AccountId));
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"FacilityMaterial not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.PersonaMaterialDepositos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(FacilityMaterial entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _context.PersonaMaterialDepositos
            .FirstOrDefault(pmd => pmd.IdPersona == GuidLongConverter.GuidToString(entity.PersonId) 
                && pmd.IdMaterial == GuidLongConverter.ToLong(entity.MaterialId)
                && pmd.IdDeposito == GuidLongConverter.ToLong(entity.FacilityId)
                && pmd.IdCuenta == GuidLongConverter.ToLong(entity.AccountId));
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"FacilityMaterial not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.PersonaMaterialDepositos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<FacilityMaterial, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
        if (predicate == null)
        {
            return await _context.PersonaMaterialDepositos
                .Where(pmd => pmd.Activo && pmd.IdCuenta == accountIdLong)
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
}

