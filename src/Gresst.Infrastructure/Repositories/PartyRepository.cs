using Gresst.Domain.Common;
using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Gresst.Infrastructure.Repositories;

public class PartyRepository : IPartyRepository<Party>
{
    private readonly InfrastructureDbContext _context;
    private readonly PartyMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    // Infrastructure/Data/Models/PersonaDb.cs

    private static string EncodeCreatedAtCursor(DateTime createdAt)
    {
        return Convert.ToBase64String(
            Encoding.UTF8.GetBytes(createdAt.ToString("O")));
    }

    private static DateTime DecodeCreatedAtCursor(string cursor)
    {
        var raw = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
        return DateTime.Parse(raw, null, DateTimeStyles.RoundtripKind);
    }

    public PartyRepository(InfrastructureDbContext context, PartyMapper mapper, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<Party>> GetAllAsync(string? partyId, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? (long?)null : long.Parse(accountId);

        var dbEntities = await _context.PersonaContactos
            .Where(pc => pc.Activo && pc.IdCuenta == accountIdLong)
            .Join(
                _context.Personas,
                pc => pc.IdContacto,
                p => p.IdPersona,
                (pc, p) => new PersonaDb
                {
                    IdPersona = pc.IdContacto,
                    IdCategoria = p.IdCategoria,
                    IdCuenta = pc.IdCuenta,
                    IdTipoIdentificacion = p.IdTipoIdentificacion,
                    IdRol = pc.IdRelacion,
                    IdTipoPersona = p.IdTipoPersona,
                    Identificacion = p.Identificacion,
                    DigitoVerificacion = p.DigitoVerificacion,
                    Nombre = pc.Nombre ?? p.Nombre,
                    Direccion = pc.Direccion ?? p.Direccion,
                    Telefono = pc.Telefono ?? p.Telefono,
                    Telefono2 = pc.Telefono2 ?? p.Telefono2,
                    Correo = pc.Correo ?? p.Correo,
                    UbicacionMapa = p.UbicacionMapa,
                    UbicacionLocal = p.UbicacionLocal,
                    Activo = pc.Activo,
                    Licencia = p.Licencia,
                    Cargo = pc.Cargo ?? p.Cargo,
                    Pagina = pc.Pagina ?? p.Pagina,
                    Firma = pc.Firma ?? p.Firma,
                    IdLocalizacion = pc.IdLocalizacion ?? p.IdLocalizacion,
                    DatosAdicionales = pc.DatosAdicionales ?? p.DatosAdicionales,
                    IdUsuarioCreacion = pc.IdUsuarioCreacion,
                    FechaCreacion = pc.FechaCreacion,
                    IdUsuarioUltimaModificacion = pc.IdUsuarioUltimaModificacion,
                    FechaUltimaModificacion = pc.FechaUltimaModificacion,
                })
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }
    public async Task<Party?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var dbEntity = await _context.Personas.FindAsync(new object[] { id }, cancellationToken);

        return dbEntity != null ? _mapper.ToDomain(PersonaDb.FromPersona(dbEntity)) : null;
    }
    public async Task<IEnumerable<Party>> FindAsync(Expression<Func<Party, bool>> predicate, string? partyId, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(partyId, cancellationToken);
        return all.Where(predicate.Compile());
    }
    public async Task<(IEnumerable<Party> Items, string? Next)> FindPagedAsync(Expression<Func<Party, bool>>? predicate, string? partyId = null, int limit = 50, string? next = null, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(partyId, cancellationToken);

        if (predicate != null)
            all = all.Where(predicate.Compile());

        // Cursor antes de ordenar
        if (!string.IsNullOrEmpty(next))
        {
            var createdAt = DecodeCreatedAtCursor(next);
            all = all.Where(e => e.CreatedAt < createdAt);
        }

        var items = all.OrderByDescending(e => e.CreatedAt)
                       .Take(limit + 1)
                       .ToList();

        var hasMore = items.Count > limit;
        if (hasMore) items = items.Take(limit).ToList();

        var nextCursor = hasMore ? EncodeCreatedAtCursor(items.Last().CreatedAt) : null;

        return (items, nextCursor);
    }

    public async Task<int> CountAsync(Expression<Func<Party, bool>>? predicate = null, string? parentId = null, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? (long?)null : long.Parse(accountId);

        if (predicate == null)
        {
            return await _context.Personas
                .Where(p => p.Activo && p.IdCuenta == accountIdLong)
                .CountAsync(cancellationToken);
        }

        var all = await GetAllAsync(parentId, cancellationToken);
        return all.Count(predicate.Compile());
    }

    public async Task<Party> AddAsync(Party entity, string? partyId, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = long.TryParse(_currentUserService.GetCurrentUserId(), out var value) ? value : 0;
        dbEntity.IdCuenta = long.TryParse(_currentUserService.GetCurrentAccountId(), out var value2) ? value2 : 0;
        
        // Generate IdPersona if empty
        if (string.IsNullOrEmpty(dbEntity.IdPersona))
        {
            dbEntity.IdPersona = Guid.NewGuid().ToString("N")[..40];
        }
        
        await _context.Personas.AddAsync(dbEntity.ToPersona(), cancellationToken);
        
        entity.Id = dbEntity.IdPersona;
        return entity;
    }
    public Task UpdateAsync(Party entity, string? partyId = null, CancellationToken cancellationToken = default)
    {
        var dbEntity = _context.Personas.Find(entity.Id);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Party with ID {entity.Id} not found");

        var personaDb = PersonaDb.FromPersona(dbEntity);
        _mapper.UpdateDatabase(entity, personaDb);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = long.TryParse(_currentUserService.GetCurrentUserId(), out var value) ? value : 0;
        
        _context.Personas.Update(dbEntity);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(Party entity, string? partyId = null, CancellationToken cancellationToken = default)
    {
        var dbEntity = _context.Personas.Find(entity.Id);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Party with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = long.TryParse(_currentUserService.GetCurrentUserId(), out var value) ? value : 0;
        
        _context.Personas.Update(dbEntity);
        return Task.CompletedTask;
    }
}

