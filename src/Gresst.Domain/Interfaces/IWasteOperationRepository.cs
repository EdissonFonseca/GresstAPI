using Gresst.Domain.Entities;

namespace Gresst.Domain.Interfaces;

/// <summary>
/// Persists operations (Relocation, Transfer, Storage) triggered by process events.
/// </summary>
public interface IWasteOperationRepository
{
    Task AddAsync(WasteOperation operation, CancellationToken ct = default);
}
