using Gresst.Application.DTOs;
using Gresst.Application.Queries;
using Gresst.Domain.Entities;
using System.Linq.Expressions;

namespace Gresst.Application.Services;

public interface IPartyService
{
    // GetAll ahora filtra automáticamente por usuario actual
    Task<IEnumerable<PartyDto>> GetAllAsync(CancellationToken cancellationToken = default);
    // FindAsync permite filtrar por cualquier criterio, pero el repositorio se encargará de aplicar el filtro de usuario actual
    Task<IEnumerable<PartyDto>> FindAsync(Expression<Func<Party, bool>> predicate, CancellationToken cancellationToken = default);

    // GetById verifica que el usuario tenga acceso
    Task<PartyDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
   
    
}

