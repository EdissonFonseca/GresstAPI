using Gresst.Domain.Entities;

namespace Gresst.Domain.Interfaces;

/// <summary>
/// Extended repository interface for Person with role-specific queries
/// </summary>
public interface IPersonRepository : IRepository<Person>
{
    Task<IEnumerable<Person>> GetByRoleAsync(string roleCode, CancellationToken cancellationToken = default);
    Task<Person?> GetByIdAndRoleAsync(string id, string roleCode, CancellationToken cancellationToken = default);
    Task SetPersonRoleAsync(string personId, string roleCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<Person>> GetClientsAsync(CancellationToken cancellationToken = default);
}

