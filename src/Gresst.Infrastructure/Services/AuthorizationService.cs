using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Services;

/// <summary>
/// Implementation of permission service
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly InfrastructureDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AuthorizationService(InfrastructureDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    // Options management
    public async Task<IEnumerable<OptionDto>> GetAllOptionsAsync(CancellationToken cancellationToken = default)
    {
        var opciones = await _context.Opcions.ToListAsync(cancellationToken);
        return opciones.Select(MapOptionToDto);
    }

    public async Task<OptionDto?> GetOptionByIdAsync(string optionId, CancellationToken cancellationToken = default)
    {
        var opcion = await _context.Opcions.FindAsync(new object[] { optionId }, cancellationToken);
        return opcion != null ? MapOptionToDto(opcion) : null;
    }

    public async Task<IEnumerable<OptionDto>> GetOptionsByParentAsync(string? parentId, CancellationToken cancellationToken = default)
    {
        var opciones = await _context.Opcions
            .Where(o => o.IdOpcionSuperior == parentId)
            .ToListAsync(cancellationToken);
        
        return opciones.Select(MapOptionToDto);
    }

    // User permissions
    public async Task<IEnumerable<UserPermissionDto>> GetUserPermissionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        
        var permissions = await _context.UsuarioOpcions
            .Include(uo => uo.IdOpcionNavigation)
            .Where(uo => uo.IdUsuario == userIdLong)
            .ToListAsync(cancellationToken);

        return permissions.Select(MapPermissionToDto);
    }

    public async Task<UserPermissionDto?> GetUserPermissionAsync(string userId, string optionId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        
        var permission = await _context.UsuarioOpcions
            .Include(uo => uo.IdOpcionNavigation)
            .FirstOrDefaultAsync(uo => uo.IdUsuario == userIdLong && uo.IdOpcion == optionId, cancellationToken);

        return permission != null ? MapPermissionToDto(permission) : null;
    }

    public async Task<bool> AssignPermissionAsync(AssignPermissionDto dto, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(dto.UserId);
        
        // Check if already exists
        var existing = await _context.UsuarioOpcions
            .FirstOrDefaultAsync(uo => uo.IdUsuario == userIdLong && uo.IdOpcion == dto.OptionId, cancellationToken);

        if (existing != null)
        {
            // Update existing
            existing.Habilitado = dto.IsEnabled;
            existing.Permisos = PermissionHelper.ToString(dto.Permissions);
            existing.Settings = dto.Settings;
        }
        else
        {
            // Create new
            var newPermission = new UsuarioOpcion
            {
                IdUsuario = userIdLong,
                IdOpcion = dto.OptionId,
                Habilitado = dto.IsEnabled,
                Permisos = PermissionHelper.ToString(dto.Permissions),
                Settings = dto.Settings,
                IdUsuarioCreacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId()),
                FechaCreacion = DateTime.UtcNow
            };

            await _context.UsuarioOpcions.AddAsync(newPermission, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RevokePermissionAsync(string userId, string optionId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        
        var permission = await _context.UsuarioOpcions
            .FirstOrDefaultAsync(uo => uo.IdUsuario == userIdLong && uo.IdOpcion == optionId, cancellationToken);

        if (permission == null)
            return false;

        _context.UsuarioOpcions.Remove(permission);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdatePermissionAsync(string userId, string optionId, AssignPermissionDto dto, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        
        var permission = await _context.UsuarioOpcions
            .FirstOrDefaultAsync(uo => uo.IdUsuario == userIdLong && uo.IdOpcion == optionId, cancellationToken);

        if (permission == null)
            return false;

        permission.Habilitado = dto.IsEnabled;
        permission.Permisos = PermissionHelper.ToString(dto.Permissions);
        permission.Settings = dto.Settings;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Permission checking
    public async Task<bool> UserHasPermissionAsync(string userId, string optionId, PermissionFlags permission, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        
        var userPermission = await _context.UsuarioOpcions
            .FirstOrDefaultAsync(uo => uo.IdUsuario == userIdLong && uo.IdOpcion == optionId && uo.Habilitado, cancellationToken);

        if (userPermission == null)
            return false;

        return PermissionHelper.HasPermission(userPermission.Permisos, permission);
    }

    public async Task<bool> CurrentUserHasPermissionAsync(string optionId, PermissionFlags permission, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        return await UserHasPermissionAsync(currentUserId, optionId, permission, cancellationToken);
    }

    // Helper mappers
    private OptionDto MapOptionToDto(Opcion opcion)
    {
        return new OptionDto
        {
            Id = opcion.IdOpcion,
            ParentId = opcion.IdOpcionSuperior,
            Description = opcion.Descripcion,
            IsConfigurable = opcion.Configurable,
            Settings = opcion.Settings
        };
    }

    private UserPermissionDto MapPermissionToDto(UsuarioOpcion usuarioOpcion)
    {
        return new UserPermissionDto
        {
            OptionId = usuarioOpcion.IdOpcion,
            OptionDescription = usuarioOpcion.IdOpcionNavigation?.Descripcion ?? usuarioOpcion.IdOpcion,
            IsEnabled = usuarioOpcion.Habilitado,
            Permissions = PermissionHelper.Parse(usuarioOpcion.Permisos),
            Settings = usuarioOpcion.Settings
        };
    }
}

