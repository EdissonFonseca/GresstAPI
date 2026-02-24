using Gresst.Application.DTOs;
using Gresst.Domain.Entities;
using System.Linq.Expressions;

namespace Gresst.Application.Services.Interfaces;

public interface IPartyService
{
    // GetAll ahora filtra automáticamente por usuario actual
    Task<IEnumerable<PartyRelatedDto>> GetAllAsync(string? partyId = null, CancellationToken cancellationToken = default);
    // FindAsync permite filtrar por cualquier criterio, pero el repositorio se encargará de aplicar el filtro de usuario actual
    Task<IEnumerable<PartyRelatedDto>> FindAsync(Expression<Func<Party, bool>> predicate, string? partyId = null, CancellationToken cancellationToken = default);

    // Paged find with cursor-based pagination and simple filters
    Task<(IEnumerable<PartyRelatedDto> Items, string? Next)> FindPagedAsync(Expression<Func<Party, bool>>? predicate = null, string? partyId = null, int limit = 50, string? next = null, CancellationToken cancellationToken = default);

    // GetById verifica que el usuario tenga acceso
    Task<PartyRelatedDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
   
    
}

