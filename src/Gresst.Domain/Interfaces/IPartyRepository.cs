using Gresst.Domain.Entities;

namespace Gresst.Domain.Interfaces;

/// <summary>
/// Extended repository interface for Person with role-specific queries
/// </summary>
public interface IPartyRepository : IRepository<Party>
{
    Task<IEnumerable<Party>> GetByRoleAsync(string roleCode, CancellationToken cancellationToken = default);
    Task<Party?> GetByIdAndRoleAsync(string id, string roleCode, CancellationToken cancellationToken = default);
    Task SetPersonRoleAsync(string personId, string roleCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Party>> GetCustomersAsync(CancellationToken cancellationToken = default);
}

