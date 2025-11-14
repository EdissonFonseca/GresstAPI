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

    // Facilities
    public async Task<bool> UserHasAccessToFacilityAsync(Guid facilityId, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = GuidLongConverter.ToLong(userId);
        var facilityIdLong = GuidLongConverter.ToLong(facilityId);

        // Admin tiene acceso a todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
            return true;

        // Verificar en UsuarioDeposito
        var hasAccess = await _context.UsuarioDepositos
            .AnyAsync(ud => ud.IdUsuario == userIdLong && ud.IdDeposito == facilityIdLong, cancellationToken);

        return hasAccess;
    }

    public async Task<IEnumerable<Guid>> GetUserFacilityIdsAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = GuidLongConverter.ToLong(userId);

        // Admin puede ver todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
        {
            var allFacilities = await _context.Depositos
                .Select(d => d.IdDeposito)
                .ToListAsync(cancellationToken);
            
            return allFacilities.Select(GuidLongConverter.ToGuid);
        }

        // Usuario normal solo ve sus facilities asignados
        var facilityIds = await _context.UsuarioDepositos
            .Where(ud => ud.IdUsuario == userIdLong)
            .Select(ud => ud.IdDeposito)
            .ToListAsync(cancellationToken);

        return facilityIds.Select(GuidLongConverter.ToGuid);
    }

    public async Task<bool> AssignFacilityToUserAsync(Guid userId, Guid facilityId, CancellationToken cancellationToken = default)
    {
        var userIdLong = GuidLongConverter.ToLong(userId);
        var facilityIdLong = GuidLongConverter.ToLong(facilityId);
        var currentUserIdLong = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());

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

    public async Task<bool> RevokeFacilityFromUserAsync(Guid userId, Guid facilityId, CancellationToken cancellationToken = default)
    {
        var userIdLong = GuidLongConverter.ToLong(userId);
        var facilityIdLong = GuidLongConverter.ToLong(facilityId);

        var assignment = await _context.UsuarioDepositos
            .FirstOrDefaultAsync(ud => ud.IdUsuario == userIdLong && ud.IdDeposito == facilityIdLong, cancellationToken);

        if (assignment == null)
            return false;

        _context.UsuarioDepositos.Remove(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    // Vehicles
    public async Task<bool> UserHasAccessToVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = GuidLongConverter.ToLong(userId);
        var vehicleIdString = GuidLongConverter.GuidToString(vehicleId);

        // Admin tiene acceso a todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
            return true;

        // Verificar en UsuarioVehiculo
        var hasAccess = await _context.UsuarioVehiculos
            .AnyAsync(uv => uv.IdUsuario == userIdLong && uv.IdVehiculo == vehicleIdString, cancellationToken);

        return hasAccess;
    }

    public async Task<IEnumerable<Guid>> GetUserVehicleIdsAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = GuidLongConverter.ToLong(userId);

        // Admin puede ver todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
        {
            var allVehicles = await _context.Vehiculos
                .Select(v => v.IdVehiculo)
                .ToListAsync(cancellationToken);
            
            return allVehicles.Select(GuidLongConverter.StringToGuid);
        }

        // Usuario normal solo ve sus vehículos asignados
        var vehicleIds = await _context.UsuarioVehiculos
            .Where(uv => uv.IdUsuario == userIdLong)
            .Select(uv => uv.IdVehiculo)
            .ToListAsync(cancellationToken);

        return vehicleIds.Select(GuidLongConverter.StringToGuid);
    }

    public async Task<bool> AssignVehicleToUserAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var userIdLong = GuidLongConverter.ToLong(userId);
        var vehicleIdString = GuidLongConverter.GuidToString(vehicleId);
        var currentUserIdLong = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());

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

    public async Task<bool> RevokeVehicleFromUserAsync(Guid userId, Guid vehicleId, CancellationToken cancellationToken = default)
    {
        var userIdLong = GuidLongConverter.ToLong(userId);
        var vehicleIdString = GuidLongConverter.GuidToString(vehicleId);

        var assignment = await _context.UsuarioVehiculos
            .FirstOrDefaultAsync(uv => uv.IdUsuario == userIdLong && uv.IdVehiculo == vehicleIdString, cancellationToken);

        if (assignment == null)
            return false;

        _context.UsuarioVehiculos.Remove(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    // Materials
    public async Task<bool> UserHasAccessToMaterialAsync(Guid materialId, CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = GuidLongConverter.ToLong(userId);
        var materialIdLong = GuidLongConverter.ToLong(materialId);

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

    public async Task<IEnumerable<Guid>> GetUserMaterialIdsAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = GuidLongConverter.ToLong(userId);

        // Admin puede ver todo
        if (await CurrentUserIsAdminAsync(cancellationToken))
        {
            var allMaterials = await _context.Materials
                .Where(m => m.Activo)
                .Select(m => m.IdMaterial)
                .ToListAsync(cancellationToken);
            
            return allMaterials.Select(GuidLongConverter.ToGuid);
        }

        // Usuario normal: materiales públicos + materiales que creó
        var materialIds = await _context.Materials
            .Where(m => m.Activo && (m.Publico || m.IdUsuarioCreacion == userIdLong))
            .Select(m => m.IdMaterial)
            .ToListAsync(cancellationToken);

        return materialIds.Select(GuidLongConverter.ToGuid);
    }

    public async Task<bool> AssignMaterialToUserAsync(Guid userId, Guid materialId, CancellationToken cancellationToken = default)
    {
        // Por ahora, la asignación se hace cambiando el material a público
        // O se puede crear una tabla UsuarioMaterial en el futuro
        var materialIdLong = GuidLongConverter.ToLong(materialId);
        var material = await _context.Materials.FindAsync(new object[] { materialIdLong }, cancellationToken);
        
        if (material == null)
            return false;

        // Hacer el material público para que todos puedan verlo
        material.Publico = true;
        material.FechaUltimaModificacion = DateTime.UtcNow;
        material.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RevokeMaterialFromUserAsync(Guid userId, Guid materialId, CancellationToken cancellationToken = default)
    {
        // Por ahora, la revocación se hace cambiando el material a privado
        var materialIdLong = GuidLongConverter.ToLong(materialId);
        var material = await _context.Materials.FindAsync(new object[] { materialIdLong }, cancellationToken);
        
        if (material == null)
            return false;

        // Hacer el material privado (solo el creador puede verlo)
        material.Publico = false;
        material.FechaUltimaModificacion = DateTime.UtcNow;
        material.IdUsuarioUltimaModificacion = GuidLongConverter.ToLong(_currentUserService.GetCurrentUserId());
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Generic
    public async Task<bool> CurrentUserIsAdminAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var userIdLong = GuidLongConverter.ToLong(userId);

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

