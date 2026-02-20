using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using System.Linq.Expressions;

namespace Gresst.Application.Services;

public interface IPartyService
{
    // GetAll ahora filtra automáticamente por usuario actual
    Task<IEnumerable<PartyDto>> GetAllAsync(string? partyId = null, CancellationToken cancellationToken = default);
    // FindAsync permite filtrar por cualquier criterio, pero el repositorio se encargará de aplicar el filtro de usuario actual
    Task<IEnumerable<PartyDto>> FindAsync(Expression<Func<Party, bool>> predicate, string? partyId = null, CancellationToken cancellationToken = default);

    // Paged find with cursor-based pagination and simple filters
    Task<(IEnumerable<PartyDto> Items, string? Next)> FindPagedAsync(Expression<Func<Party, bool>>? predicate = null, string? partyId = null, int limit = 50, string? next = null, CancellationToken cancellationToken = default);

    // GetById verifica que el usuario tenga acceso
    Task<PartyDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
   
    
}

