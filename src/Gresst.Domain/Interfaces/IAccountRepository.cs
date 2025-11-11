using Gresst.Domain.Entities;

namespace Gresst.Domain.Interfaces;

/// <summary>
/// Specific repository for Account with additional methods beyond IRepository
/// </summary>
public interface IAccountRepository : IRepository<Account>
{
    // Método específico adicional
    Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Account?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
