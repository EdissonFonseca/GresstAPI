using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gresst.Infrastructure.Services;

/// <summary>
/// Implementation of data segmentation service
/// Filters resources based on user assignments (UsuarioDeposito, UsuarioVehiculo, etc.)
/// </summary>
public class DataSegmentationService : IDataSegmentationService
{
    private readonly InfrastructureDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DataSegmentationService(InfrastructureDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    // Facilities - facility Id is string (BaseEntity.Id)
    public async Task<bool> UserHasAccessToFacilityAsync(string facilityId, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = IdConversion.ToLongFromString(userId);
        var facilityIdLong = string.IsNullOrEmpty(facilityId) ? 0L : long.Parse(facilityId);

        // Admin tiene acceso a todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
            return true;

        // Verificar en UsuarioDeposito
        var hasAccess = await _context.UsuarioDepositos
            .AnyAsync(ud => ud.IdUsuario == userIdLong && ud.IdDeposito == facilityIdLong, cancellationToken);

        return hasAccess;
    }

    public async Task<IEnumerable<string>> GetUserFacilityIdsAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = IdConversion.ToLongFromString(userId);

        // Admin puede ver todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
        {
            var allFacilities = await _context.Depositos
                .Select(d => d.IdDeposito)
                .ToListAsync(cancellationToken);
            
            return allFacilities.Select(x => x.ToString());
        }

        // Usuario normal solo ve sus facilities asignados
        var facilityIds = await _context.UsuarioDepositos
            .Where(ud => ud.IdUsuario == userIdLong)
            .Select(ud => ud.IdDeposito)
            .ToListAsync(cancellationToken);

        return facilityIds.Select(x => x.ToString());
    }

    public async Task<bool> AssignFacilityToUserAsync(string userId, string facilityId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        var facilityIdLong = IdConversion.ToLongFromString(facilityId);
        var currentUserIdLong = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());

        // Verificar si ya existe
        var exists = await _context.UsuarioDepositos
            .AnyAsync(ud => ud.IdUsuario == userIdLong && ud.IdDeposito == facilityIdLong, cancellationToken);

        if (exists)
            return false;

        var assignment = new UsuarioDeposito
        {
            IdUsuario = userIdLong,
            IdDeposito = facilityIdLong,
            IdUsuarioCreacion = currentUserIdLong,
            FechaCreacion = DateTime.UtcNow
        };

        await _context.UsuarioDepositos.AddAsync(assignment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<bool> RevokeFacilityFromUserAsync(string userId, string facilityId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        var facilityIdLong = IdConversion.ToLongFromString(facilityId);

        var assignment = await _context.UsuarioDepositos
            .FirstOrDefaultAsync(ud => ud.IdUsuario == userIdLong && ud.IdDeposito == facilityIdLong, cancellationToken);

        if (assignment == null)
            return false;

        _context.UsuarioDepositos.Remove(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    // Vehicles - vehicle Id is string
    public async Task<bool> UserHasAccessToVehicleAsync(string vehicleId, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = IdConversion.ToLongFromString(userId);
        var vehicleIdString = vehicleId ?? string.Empty;

        // Admin tiene acceso a todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
            return true;

        // Verificar en UsuarioVehiculo
        var hasAccess = await _context.UsuarioVehiculos
            .AnyAsync(uv => uv.IdUsuario == userIdLong && uv.IdVehiculo == vehicleIdString, cancellationToken);

        return hasAccess;
    }

    public async Task<IEnumerable<string>> GetUserVehicleIdsAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = IdConversion.ToLongFromString(userId);

        // Admin puede ver todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
        {
            var allVehicles = await _context.Vehiculos
                .Select(v => v.IdVehiculo)
                .ToListAsync(cancellationToken);
            
            return allVehicles.Where(x => x != null).Select(x => x!).ToList();
        }

        // Usuario normal solo ve sus vehículos asignados
        var vehicleIds = await _context.UsuarioVehiculos
            .Where(uv => uv.IdUsuario == userIdLong)
            .Select(uv => uv.IdVehiculo)
            .ToListAsync(cancellationToken);

        return vehicleIds.Where(x => x != null).Select(x => x!).ToList();
    }

    public async Task<bool> AssignVehicleToUserAsync(string userId, string vehicleId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        var vehicleIdString = vehicleId ?? string.Empty;
        var currentUserIdLong = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());

        var exists = await _context.UsuarioVehiculos
            .AnyAsync(uv => uv.IdUsuario == userIdLong && uv.IdVehiculo == vehicleIdString, cancellationToken);

        if (exists)
            return false;

        var assignment = new UsuarioVehiculo
        {
            IdUsuario = userIdLong,
            IdVehiculo = vehicleIdString,
            IdUsuarioCreacion = currentUserIdLong,
            FechaCreacion = DateTime.UtcNow
        };

        await _context.UsuarioVehiculos.AddAsync(assignment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<bool> RevokeVehicleFromUserAsync(string userId, string vehicleId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        var vehicleIdString = vehicleId ?? string.Empty;

        var assignment = await _context.UsuarioVehiculos
            .FirstOrDefaultAsync(uv => uv.IdUsuario == userIdLong && uv.IdVehiculo == vehicleIdString, cancellationToken);

        if (assignment == null)
            return false;

        _context.UsuarioVehiculos.Remove(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    // Materials - material Id is string (BD uses long)
    public async Task<bool> UserHasAccessToMaterialAsync(string materialId, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = IdConversion.ToLongFromString(userId);
        var materialIdLong = string.IsNullOrEmpty(materialId) ? 0L : long.Parse(materialId);

        // Admin tiene acceso a todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
            return true;

        // Buscar el material
        var material = await _context.Materials.FindAsync(new object[] { materialIdLong }, cancellationToken);
        if (material == null)
            return false;

        // Si es público, todos pueden verlo
        if (material.Publico)
            return true;

        // Si es privado, solo el creador puede verlo
        return material.IdUsuarioCreacion == userIdLong;
    }

    public async Task<IEnumerable<string>> GetUserMaterialIdsAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = IdConversion.ToLongFromString(userId);

        // Admin puede ver todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
        {
            var allMaterials = await _context.Materials
                .Where(m => m.Activo)
                .Select(m => m.IdMaterial)
                .ToListAsync(cancellationToken);
            
            return allMaterials.Select(x => x.ToString()).ToList();
        }

        // Usuario normal: materiales públicos + materiales que creó
        var materialIds = await _context.Materials
            .Where(m => m.Activo && (m.Publico || m.IdUsuarioCreacion == userIdLong))
            .Select(m => m.IdMaterial)
            .ToListAsync(cancellationToken);

        return materialIds.Select(x => x.ToString()).ToList();
    }

    public async Task<bool> AssignMaterialToUserAsync(string userId, string materialId, CancellationToken cancellationToken = default)
    {
        // Por ahora, la asignación se hace cambiando el material a público
        // O se puede crear una tabla UsuarioMaterial en el futuro
        var materialIdLong = IdConversion.ToLongFromString(materialId);
        var material = await _context.Materials.FindAsync(new object[] { materialIdLong }, cancellationToken);
        
        if (material == null)
            return false;

        // Hacer el material público para que todos puedan verlo
        material.Publico = true;
        material.FechaUltimaModificacion = DateTime.UtcNow;
        material.IdUsuarioUltimaModificacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RevokeMaterialFromUserAsync(string userId, string materialId, CancellationToken cancellationToken = default)
    {
        // Por ahora, la revocación se hace cambiando el material a privado
        var materialIdLong = IdConversion.ToLongFromString(materialId);
        var material = await _context.Materials.FindAsync(new object[] { materialIdLong }, cancellationToken);
        
        if (material == null)
            return false;

        // Hacer el material privado (solo el creador puede verlo)
        material.Publico = false;
        material.FechaUltimaModificacion = DateTime.UtcNow;
        material.IdUsuarioUltimaModificacion = IdConversion.ToLongFromString(_currentUserService.GetCurrentUserId());
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Generic
    public async Task<bool> CurrentUserIsAdminAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = IdConversion.ToLongFromString(userId);

        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        
        if (usuario == null)
            return false;

        // Verificar roles en DatosAdicionales
        if (!string.IsNullOrEmpty(usuario.DatosAdicionales))
        {
            try
            {
                var json = System.Text.Json.JsonDocument.Parse(usuario.DatosAdicionales);
                if (json.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    var rolesJson = rolesElement.GetRawText();
                    return rolesJson.Contains("Admin", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch { }
        }

        return false;
    }
}

