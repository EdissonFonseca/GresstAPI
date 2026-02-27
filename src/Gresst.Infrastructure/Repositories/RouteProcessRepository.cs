using Gresst.Domain.RouteProcesses;
using Gresst.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Repositories;

public class RouteProcessRepository : IRouteProcessRepository
{
    private readonly RouteProcessDbContext _context;

    public RouteProcessRepository(RouteProcessDbContext context)
        => _context = context;

    public async Task<RouteProcess?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.RouteProcesses
            .Include(r => r.Stops)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<RouteProcess>> GetByStatusAsync(
        RouteStatus status, CancellationToken ct = default) =>
        await _context.RouteProcesses
            .Include(r => r.Stops)
            .Where(r => r.Status == status)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<RouteProcess>> GetByVehicleIdAsync(
        Guid vehicleId, CancellationToken ct = default) =>
        await _context.RouteProcesses
            .Include(r => r.Stops)
            .Where(r => r.VehicleId == vehicleId)
            .ToListAsync(ct);

    public async Task AddAsync(RouteProcess routeProcess, CancellationToken ct = default)
    {
        await _context.RouteProcesses.AddAsync(routeProcess, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RouteProcess routeProcess, CancellationToken ct = default)
    {
        _context.RouteProcesses.Update(routeProcess);
        await _context.SaveChangesAsync(ct);
    }
}
