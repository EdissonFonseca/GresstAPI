using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DomainRoute = Gresst.Domain.Entities.Route;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Route with automatic mapping to/from Rutum (BD)
/// Handles VehicleId conversion: Domain uses Guid? (optional), BD uses string (mandatory)
/// </summary>
public class RouteRepository : IRepository<DomainRoute>
{
    private readonly InfrastructureDbContext _context;
    private readonly RouteMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public RouteRepository(
        InfrastructureDbContext context, 
        RouteMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<DomainRoute?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id) || !long.TryParse(id, out var idLong))
            return null;
        var dbEntity = await _context.Ruta
            .Include(r => r.IdVehiculoNavigation)
            .Include(r => r.IdResponsableNavigation)
            .Include(r => r.RutaDepositos)
                .ThenInclude(rd => rd.IdDepositoNavigation)
            .FirstOrDefaultAsync(r => r.IdRuta == idLong, cancellationToken);
        
        if (dbEntity == null)
            return null;

        var route = _mapper.ToDomain(dbEntity);
        
        // Try to resolve VehicleId from IdVehiculo (LicensePlate)
        // Note: Vehicle lookup will be done in the service layer if needed
        // For now, we store the LicensePlate in a way that can be resolved later
        
        return route;
    }

    public async Task<IEnumerable<DomainRoute>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        var dbEntities = await _context.Ruta
            .Where(r => r.IdCuenta == accountIdLong)
            .Include(r => r.IdVehiculoNavigation)
            .Include(r => r.IdResponsableNavigation)
            .Include(r => r.RutaDepositos)
                .ThenInclude(rd => rd.IdDepositoNavigation)
            .ToListAsync(cancellationToken);

        var routes = dbEntities.Select(_mapper.ToDomain).ToList();
        
        // Note: Vehicle lookup will be done in the service layer if needed
        
        return routes;
    }

    public async Task<IEnumerable<DomainRoute>> FindAsync(Expression<Func<DomainRoute, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // Para queries más complejas, traemos todo y filtramos en memoria
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<DomainRoute> AddAsync(DomainRoute entity, CancellationToken cancellationToken = default)
    {
        // Note: Vehicle should be loaded before calling AddAsync if VehicleId is provided
        // The service layer will handle this
        
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        dbEntity.IdCuenta = string.IsNullOrEmpty(_currentUserService.GetCurrentAccountId()) ? 0L : long.Parse(_currentUserService.GetCurrentAccountId());
        dbEntity.Activo = true;

        await _context.Ruta.AddAsync(dbEntity, cancellationToken);
        
        // Update domain entity with generated ID
        entity.Id = dbEntity.IdRuta.ToString();
        
        return entity;
    }

    public async Task UpdateAsync(DomainRoute entity, CancellationToken cancellationToken = default)
    {
        // Note: Vehicle should be loaded before calling UpdateAsync if VehicleId is provided
        // The service layer will handle this
        
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = await _context.Ruta.FindAsync(new object[] { idLong }, cancellationToken);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Route with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        // Update audit fields
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        
        _context.Ruta.Update(dbEntity);
    }

    public Task DeleteAsync(DomainRoute entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Ruta.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Route with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        
        _context.Ruta.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<DomainRoute, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        
        if (predicate == null)
        {
            return await _context.Ruta
                .Where(r => r.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }
}

