using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DomainRouteStop = Gresst.Domain.Entities.RouteStop;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for RouteStop with automatic mapping to/from RutaDeposito (BD)
/// </summary>
public class RouteStopRepository : IRepository<DomainRouteStop>
{
    private readonly InfrastructureDbContext _context;
    private readonly RouteStopMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public RouteStopRepository(
        InfrastructureDbContext context,
        RouteStopMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<DomainRouteStop?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        // RouteStop has composite key, so GetByIdAsync needs to be adapted
        await Task.CompletedTask;
        return null;
    }

    public async Task<IEnumerable<DomainRouteStop>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        // Get all route stops for routes in this account
        var dbEntities = await _context.RutaDepositos
            .Where(rd => rd.IdRutaNavigation.IdCuenta == accountIdLong)
            .Include(rd => rd.IdRutaNavigation)
            .Include(rd => rd.IdDepositoNavigation)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<DomainRouteStop>> FindAsync(Expression<Func<DomainRouteStop, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<DomainRouteStop> AddAsync(DomainRouteStop entity, CancellationToken cancellationToken = default)
    {
        if (!entity.FacilityId.HasValue)
            throw new InvalidOperationException("RouteStop must have a FacilityId");

        var dbEntity = _mapper.ToDatabase(entity);
        
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());

        await _context.RutaDepositos.AddAsync(dbEntity, cancellationToken);
        
        return entity;
    }

    public Task UpdateAsync(DomainRouteStop entity, CancellationToken cancellationToken = default)
    {
        if (!entity.FacilityId.HasValue)
            throw new InvalidOperationException("RouteStop must have a FacilityId");

        var routeIdLong = GuidLongConverter.ToLong(entity.RouteId);
        var facilityIdLong = GuidLongConverter.ToLong(entity.FacilityId.Value);

        var dbEntity = _context.RutaDepositos.Find(routeIdLong, facilityIdLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"RouteStop for Route {entity.RouteId}, Facility {entity.FacilityId} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        _context.RutaDepositos.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(DomainRouteStop entity, CancellationToken cancellationToken = default)
    {
        if (!entity.FacilityId.HasValue)
            throw new InvalidOperationException("RouteStop must have a FacilityId");

        var routeIdLong = GuidLongConverter.ToLong(entity.RouteId);
        var facilityIdLong = GuidLongConverter.ToLong(entity.FacilityId.Value);

        var dbEntity = _context.RutaDepositos.Find(routeIdLong, facilityIdLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"RouteStop for Route {entity.RouteId}, Facility {entity.FacilityId} not found");

        // Hard delete (no Activo field in RutaDeposito)
        _context.RutaDepositos.Remove(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<DomainRouteStop, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        if (predicate == null)
        {
            return await _context.RutaDepositos
                .Where(rd => rd.IdRutaNavigation.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

