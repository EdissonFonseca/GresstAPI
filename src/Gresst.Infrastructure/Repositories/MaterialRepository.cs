using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DbMaterial = Gresst.Infrastructure.Data.Entities.Material;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Material with automatic mapping to/from Material (BD)
/// </summary>
public class MaterialRepository : IRepository<Material>
{
    private readonly InfrastructureDbContext _context;
    private readonly MaterialMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public MaterialRepository(
        InfrastructureDbContext context, 
        MaterialMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Material?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id) || !long.TryParse(id, out var idLong))
            return null;
        var dbEntity = await _context.Materials.FindAsync(new object[] { idLong }, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Material>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accountId = _currentUserService.GetCurrentAccountId();
        var accountPersonId = _currentUserService.GetCurrentAccountPersonId();
        var accountIdLong = string.IsNullOrEmpty(accountId) ? 0L : long.Parse(accountId);
        var accountPersonIdString = GuidStringConverter.ToString(accountPersonId);

        var dbEntities = await (
            from m in _context.Materials
                .Include(x => x.IdTipoResiduoNavigation)
            join pm in _context.PersonaMaterials
                on m.IdMaterial equals pm.IdMaterial
            where m.Activo
                  && pm.IdCuenta == accountIdLong
                  && pm.IdPersona == accountPersonIdString
            select new DbMaterial
            {
                IdMaterial = m.IdMaterial,

                Nombre = pm.Nombre ?? m.Nombre,
                PrecioServicio = pm.PrecioServicio ?? m.PrecioServicio,
                PrecioCompra = pm.PrecioCompra ?? m.PrecioCompra,
                Peso = pm.Peso ?? m.Peso,
                Volumen = pm.Volumen ?? m.Volumen,
                FactorCompensacionEmision = pm.FactorCompensacionEmision ?? m.FactorCompensacionEmision,

                Descripcion = m.Descripcion,
                Sinonimos = m.Sinonimos,
                IdTipoResiduo = m.IdTipoResiduo,
                Imagen = m.Imagen,
                Medicion = m.Medicion,
                Publico = m.Publico,
                Aprovechable = m.Aprovechable,
                Activo = m.Activo,
                IdUsuarioCreacion = m.IdUsuarioCreacion,
                FechaCreacion = m.FechaCreacion,
                IdUsuarioUltimaModificacion = m.IdUsuarioUltimaModificacion,
                FechaUltimaModificacion = m.FechaUltimaModificacion,

                // NECESARIO para que el mapeo funcione si quieres usar navegación
                IdTipoResiduoNavigation = m.IdTipoResiduoNavigation
            })
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Material>> FindAsync(Expression<Func<Material, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // Para queries más complejas, necesitarías traducir el Expression
        // Por ahora, traemos todo y filtramos en memoria (menos eficiente pero funcional)
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Material> AddAsync(Material entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        
        await _context.Materials.AddAsync(dbEntity, cancellationToken);
        
        // Return domain entity with generated ID
        entity.Id = dbEntity.IdMaterial.ToString();
        return entity;
    }

    public Task UpdateAsync(Material entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Materials.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Material with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        // Update audit fields
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Materials.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Material entity, CancellationToken cancellationToken = default)
    {
        var idLong = string.IsNullOrEmpty(entity.Id) ? 0L : long.Parse(entity.Id);
        var dbEntity = _context.Materials.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Material with ID {entity.Id} not found");

        // Soft delete
        dbEntity.Activo = false;
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Materials.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Material, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _context.Materials
                .Where(m => m.Activo)
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }

    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return GuidLongConverter.ToLong(userId);
    }
}

