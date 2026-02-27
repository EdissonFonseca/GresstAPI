using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

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

    private async Task<List<long>> GetMaterialIdsPorDepositoRecursivoAsync(
        string partyId,
        long? accountIdLong,
        long depositoId,
        CancellationToken cancellationToken)
    {
        // Buscar materiales en este depósito
        var materialesIds = await _context.PersonaMaterialDepositos
            .Where(m => m.IdPersona == partyId
                     && m.IdCuenta == accountIdLong
                     && m.IdDeposito == depositoId)
            .Select(m => m.IdMaterial)
            .ToListAsync(cancellationToken);

        if (materialesIds.Any())
            return materialesIds;

        // Si no encontró, buscar el depósito padre
        var depositoPadreId = await _context.Depositos
            .Where(d => d.IdDeposito == depositoId)
            .Select(d => d.IdSuperior) // ajusta al nombre real del campo
            .FirstOrDefaultAsync(cancellationToken);

        // Si no tiene padre, terminar la recursión
        if (depositoPadreId == null || depositoPadreId == 0)
            return new List<long>();

        // Subir al padre y repetir
        return await GetMaterialIdsPorDepositoRecursivoAsync(
            partyId,
            accountIdLong,
            depositoPadreId.Value,
            cancellationToken);
    }
    private async Task<List<Waste>> GetMaterialesResueltosAsync(string partyId, long depositoId, long ubicacionId, CancellationToken cancellationToken)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = Convert.ToInt16(accountId);

        // Level 1 — by facility (recursive by parent)
        var materialesIds = await GetMaterialIdsPorDepositoRecursivoAsync(partyId, accountIdLong, depositoId, cancellationToken);

        //// Nivel 2 — por sede
        //if (!materialesIds.Any())
        //{
        //    materialesIds = await _context.PersonaMaterialUbicaciones
        //        .Where(m => m.IdPersona == partyId
        //                 && m.IdCuenta == accountIdLong
        //                 && m.IdUbicacion == ubicacionId)
        //        .Select(m => m.IdMaterial)
        //        .ToListAsync(cancellationToken);
        //}

        // Level 3 — party
        if (!materialesIds.Any())
        {
            materialesIds = await _context.PersonaMaterials
                .Where(m => m.IdPersona == partyId
                         && m.IdCuenta == accountIdLong)
                .Select(m => m.IdMaterial)
                .ToListAsync(cancellationToken);
        }

        // Nivel 4 — por gestor
        if (!materialesIds.Any())
        {
            var personId = _currentUserService.GetCurrentPersonId();
            materialesIds = await _context.PersonaMaterials
                .Where(m => m.IdPersona == personId
                         && m.IdCuenta == accountIdLong)
                .Select(m => m.IdMaterial)
                .ToListAsync(cancellationToken);
        }

        if (!materialesIds.Any())
            return new List<Waste>();

        return await _context.Materials
            .Where(m => materialesIds.Contains(m.IdMaterial))
            .Select(m => new Waste
            {
                Id = m.IdMaterial.ToString(),
                Name = m.Nombre ?? string.Empty,
            })
            .ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Party>> GetAllAsync(string? partyId, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? (long?)null : long.Parse(accountId);

        if (partyId == null)
            partyId = _currentUserService.GetCurrentAccountPersonId();

        var query1 = _context.PersonaContactos
            .Where(pc => pc.Activo && pc.IdCuenta == accountIdLong && pc.IdPersona == partyId)
            .Join(
                _context.Personas,
                pc => pc.IdContacto,
                p => p.IdPersona,
                (pc, p) => new PersonaDb
                {
                    IdPersona = pc.IdContacto,
                    IdTipoIdentificacion = p.IdTipoIdentificacion,
                    IdRelacion = pc.IdRelacion,
                    IdTipoPersona = p.IdTipoPersona,
                    Identificacion = p.Identificacion,
                    DigitoVerificacion = p.DigitoVerificacion,
                    Nombre = pc.Nombre ?? p.Nombre,
                    Direccion = pc.Direccion ?? p.Direccion,
                    Telefono = pc.Telefono ?? p.Telefono,
                    Telefono2 = pc.Telefono2 ?? p.Telefono2,
                    Correo = pc.Correo ?? p.Correo,
                    Activo = pc.Activo,
                    Firma = pc.Firma ?? p.Firma,
                    IdLocalizacion = pc.IdLocalizacion ?? p.IdLocalizacion,
                });

        var query2 = _context.PersonaContactos
            .Where(pc => pc.Activo && pc.IdCuenta == accountIdLong && pc.IdContacto == partyId && pc.IdRelacion == "CL")
            .Join(
                _context.Personas,
                pc => pc.IdPersona,
                p => p.IdPersona,
                (pc, p) => new PersonaDb
                {
                    IdPersona = pc.IdPersona,
                    IdTipoIdentificacion = p.IdTipoIdentificacion,
                    IdRelacion = "PR",
                    IdTipoPersona = p.IdTipoPersona,
                    Identificacion = p.Identificacion,
                    DigitoVerificacion = p.DigitoVerificacion,
                    Nombre = pc.Nombre ?? p.Nombre,
                    Direccion = pc.Direccion ?? p.Direccion,
                    Telefono = pc.Telefono ?? p.Telefono,
                    Telefono2 = pc.Telefono2 ?? p.Telefono2,
                    Correo = pc.Correo ?? p.Correo,
                    Activo = pc.Activo,
                    Firma = pc.Firma ?? p.Firma,
                    IdLocalizacion = pc.IdLocalizacion ?? p.IdLocalizacion,
                });

        var dbEntities = await query1.Concat(query2).ToListAsync(cancellationToken);

        var merged = dbEntities
            .GroupBy(p => p.IdPersona)
            .Select(g =>
            {
                return new PersonaDb
                {
                    IdPersona            = g.Key,
                    IdTipoIdentificacion = g.First().IdTipoIdentificacion,
                    IdTipoPersona        = g.First().IdTipoPersona,
                    Identificacion       = g.First().Identificacion,
                    DigitoVerificacion   = g.First().DigitoVerificacion,
                    Nombre               = g.Select(p => p.Nombre).FirstOrDefault(v => !string.IsNullOrEmpty(v)),
                    Direccion            = g.Select(p => p.Direccion).FirstOrDefault(v => !string.IsNullOrEmpty(v)),
                    Telefono             = g.Select(p => p.Telefono).FirstOrDefault(v => !string.IsNullOrEmpty(v)),
                    Telefono2            = g.Select(p => p.Telefono2).FirstOrDefault(v => !string.IsNullOrEmpty(v)),
                    Correo               = g.Select(p => p.Correo).FirstOrDefault(v => !string.IsNullOrEmpty(v)),
                    Firma                = g.Select(p => p.Firma).FirstOrDefault(v => !string.IsNullOrEmpty(v)),
                    IdLocalizacion       = g.Select(p => p.IdLocalizacion).FirstOrDefault(v => v != null),
                    Activo               = g.Any(p => p.Activo),
                    Relaciones = g
                        .Select(p => p.IdRelacion)
                        .Where(r => r != null)
                        .Select(r => r!)
                        .Distinct()
                        .ToList(),
                };
            })
            .ToList();

        return merged.Select(_mapper.ToDomain).ToList();
    }
    public async Task<IEnumerable<Party>> GetAllWithDetailsAsync(string? partyId, CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountIdLong = string.IsNullOrEmpty(accountId)
            ? (long?)null
            : long.Parse(accountId);

        var all = await GetAllAsync(partyId, cancellationToken);

        foreach (var party in all)
        {
            var sedes = await _context.PersonaLocalizacions
                .Where(p => p.IdCuenta == accountIdLong && p.IdPersona == party.Id)
                .ToListAsync(cancellationToken);

            foreach (var sede in sedes)
            {
                // Agregar la sede directamente al party
                var sedeDto = new Facility
                {
                    Id = sede.IdUbicacion.ToString(),
                    ParentId = null, // sede no tiene padre
                    LocalityId = sede.IdUbicacion.ToString(),
                    Name = sede.Nombre ?? string.Empty,
                    Address = sede.Direccion,
                    Phone = sede.Telefono,
                    Email = sede.Correo,
                    Reference = sede.Referencia,
                    Types = new List<FacilityType> { FacilityType.Adminstrative },
                    IsActive = sede.Activo
                };

                party.Facilities.Add(sedeDto); // ← sede al mismo nivel

                var depositoIds = await _context.PersonaLocalizacionDepositos
                    .Where(p => p.IdPersona == party.Id
                             && p.IdUbicacion == sede.IdUbicacion)
                    .Select(p => p.IdDeposito)
                    .ToListAsync(cancellationToken);

                var depositosRaw = await _context.Depositos
                    .Where(d => depositoIds.Contains(d.IdDeposito))
                    .ToListAsync(cancellationToken);

                foreach (var deposito in depositosRaw)
                {
                    var wasteTypes = await GetMaterialesResueltosAsync(
                        party.Id,
                        deposito.IdDeposito,
                        sede.IdUbicacion,
                        cancellationToken);

                    var depositoDto = new Facility
                    {
                        Id = deposito.IdDeposito.ToString(),
                        ParentId = sede.IdUbicacion.ToString(), // ← apunta a la sede
                        LocalityId = deposito.IdDeposito.ToString(),
                        Location = deposito.Ubicacion as Point,
                        Name = deposito.Nombre ?? string.Empty,
                        Address = deposito.Direccion,
                        Phone = deposito.Telefono,
                        Email = deposito.Correo,
                        Reference = deposito.Referencia,
                        IsActive = deposito.Activo,
                        Types = TypeMapper.ToFacilityTypes(deposito),
                        WasteTypes = wasteTypes
                    };

                    party.Facilities.Add(depositoDto); // ← depósito al mismo nivel
                }
            }
        }

        return all;
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
        
        // Generate IdPersona if empty
        if (string.IsNullOrEmpty(dbEntity.IdPersona))
        {
            dbEntity.IdPersona = Guid.NewGuid().ToString("N")[..40];
        }

        var persona = dbEntity.ToPersona();
        persona.FechaCreacion = DateTime.UtcNow;
        persona.IdUsuarioCreacion = long.TryParse(_currentUserService.GetCurrentUserId(), out var value) ? value : 0;
        persona.IdCuenta = long.TryParse(_currentUserService.GetCurrentAccountId(), out var value2) ? value2 : 0;

        await _context.Personas.AddAsync(persona, cancellationToken);
        
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

