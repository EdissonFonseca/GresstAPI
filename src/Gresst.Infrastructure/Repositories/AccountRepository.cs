using Gresst.Domain.Entities;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Gresst.Infrastructure.Repositories;

/// <summary>
/// Repository for Account with automatic mapping to/from Cuentum
/// Follows the same pattern as FacilityRepository
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly InfrastructureDbContext _context;
    private readonly AccountMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AccountRepository(
        InfrastructureDbContext context,
        AccountMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    // Métodos de IRepository<Account>
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(id);
        var dbEntity = await _context.Cuenta
            .Include(c => c.IdPersonaNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .FirstOrDefaultAsync(c => c.IdCuenta == idLong, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<IEnumerable<Account>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dbEntities = await _context.Cuenta
            .Include(c => c.IdPersonaNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .Where(c => c.IdEstado != "I") // No incluir inactivos
            .ToListAsync(cancellationToken);

        return dbEntities.Select(_mapper.ToDomain).ToList();
    }

    public async Task<IEnumerable<Account>> FindAsync(Expression<Func<Account, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var all = await GetAllAsync(cancellationToken);
        return all.Where(predicate.Compile());
    }

    public async Task<Account> AddAsync(Account entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.ToDatabase(entity);
        
        // Set audit fields
        dbEntity.FechaCreacion = DateTime.UtcNow;
        dbEntity.IdUsuarioCreacion = GetCurrentUserIdAsLong();
        
        await _context.Cuenta.AddAsync(dbEntity, cancellationToken);
        
        entity.Id = ConvertLongToGuid(dbEntity.IdCuenta);
        return entity;
    }

    public Task UpdateAsync(Account entity, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(entity.Id);
        var dbEntity = _context.Cuenta.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Account with ID {entity.Id} not found");

        _mapper.UpdateDatabase(entity, dbEntity);
        
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Cuenta.Update(dbEntity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Account entity, CancellationToken cancellationToken = default)
    {
        var idLong = ConvertGuidToLong(entity.Id);
        var dbEntity = _context.Cuenta.Find(idLong);
        
        if (dbEntity == null)
            throw new KeyNotFoundException($"Account with ID {entity.Id} not found");

        // Soft delete
        dbEntity.IdEstado = "I"; // Inactivo
        dbEntity.FechaUltimaModificacion = DateTime.UtcNow;
        dbEntity.IdUsuarioUltimaModificacion = GetCurrentUserIdAsLong();
        
        _context.Cuenta.Update(dbEntity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<Account, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _context.Cuenta
                .Where(c => c.IdEstado != "I")
                .CountAsync(cancellationToken);
        }
        
        var all = await GetAllAsync(cancellationToken);
        return all.Count(predicate.Compile());
    }

    // Métodos específicos de IAccountRepository
    public async Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = ConvertGuidToLong(userId);
        
        var dbEntity = await _context.Cuenta
            .Include(c => c.IdPersonaNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .FirstOrDefaultAsync(c => c.IdUsuario == userIdLong, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    public async Task<Account?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        // Si tienes un campo código en Cuentum
        var dbEntity = await _context.Cuenta
            .Include(c => c.IdPersonaNavigation)
            .Include(c => c.IdUsuarioNavigation)
            .FirstOrDefaultAsync(c => c.Nombre == code, cancellationToken);
        
        return dbEntity != null ? _mapper.ToDomain(dbEntity) : null;
    }

    // Helper methods
    private Guid ConvertLongToGuid(long id)
    {
        if (id == 0) return Guid.Empty;
        return new Guid(id.ToString().PadLeft(32, '0'));
    }

    private Guid ConvertStringToGuid(string id)
    {
        if (string.IsNullOrEmpty(id)) return Guid.Empty;
        
        if (Guid.TryParse(id, out var guid))
            return guid;
        
        var hexString = new string(id.Where(c => char.IsLetterOrDigit(c)).ToArray());
        hexString = hexString.PadLeft(32, '0').Substring(0, 32);
        
        var formatted = $"{hexString.Substring(0, 8)}-{hexString.Substring(8, 4)}-{hexString.Substring(12, 4)}-{hexString.Substring(16, 4)}-{hexString.Substring(20, 12)}";
        
        return Guid.Parse(formatted);
    }

    private long ConvertGuidToLong(Guid guid)
    {
        if (guid == Guid.Empty) return 0;
        
        var guidString = guid.ToString().Replace("-", "");
        var numericPart = new string(guidString.Where(char.IsDigit).Take(18).ToArray());
        
        return long.TryParse(numericPart, out var result) ? result : 0;
    }

    private string ConvertGuidToString(Guid guid)
    {
        if (guid == Guid.Empty) return string.Empty;
        return guid.ToString().Replace("-", "").Substring(0, 40);
    }

    private long GetCurrentUserIdAsLong()
    {
        var userId = _currentUserService.GetCurrentUserId();
        return ConvertGuidToLong(userId);
    }
}

