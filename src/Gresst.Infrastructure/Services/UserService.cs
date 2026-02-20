using Gresst.Application.Constants;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Gresst.Infrastructure.Services;

/// <summary>
/// Service for user management operations
/// </summary>
public class UserService : IUserService
{
    private readonly InfrastructureDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UserService(InfrastructureDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = long.TryParse(userId, out var value) ? value : 0;

        var usuario = await _context.Usuarios
            .Include(u => u.IdPersonaNavigation)
            .FirstOrDefaultAsync(u => u.IdUsuario == userIdLong, cancellationToken);

        if (usuario == null)
            return null;

        List<string> roles = new List<string>();

        var idCuenta = await _context.Cuenta
            .FirstOrDefaultAsync(c => c.IdCuenta == usuario.IdCuenta, cancellationToken);
        if (idCuenta != null)
            if (idCuenta.IdUsuario == usuario.IdUsuario) //Is administrator
                roles.Add(ApiRoles.Admin);

        if (roles.Count == 0)
            roles.Add(ApiRoles.User);

        return MapToDto(usuario, roles.ToArray());
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var usuario = await _context.Usuarios
            .Include(u => u.IdPersonaNavigation)
            .FirstOrDefaultAsync(u => u.Correo == email, cancellationToken);

        if (usuario == null)
            return null;

        List<string> roles = new List<string>();

        var idCuenta = await _context.Cuenta
            .FirstOrDefaultAsync(c => c.IdCuenta == usuario.IdCuenta, cancellationToken);
        if (idCuenta != null)
            if (idCuenta.IdUsuario == usuario.IdUsuario) //Is administrator
                roles.Add(ApiRoles.Admin);

        if (roles.Count == 0)
            roles.Add(ApiRoles.User);

        return MapToDto(usuario, roles.ToArray());
    }

    public async Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (string.IsNullOrEmpty(currentUserId))
            return null;

        return await GetUserByIdAsync(currentUserId, cancellationToken);
    }

    public async Task<IEnumerable<UserDto>> GetUsersByAccountAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var accountIdLong = long.TryParse(accountId, out var value) ? value : 0;
        
        var usuarios = await _context.Usuarios
            .Include(u => u.IdPersonaNavigation)
            .Where(u => u.IdCuenta == accountIdLong)
            .ToListAsync(cancellationToken);

        return usuarios.Select(u => MapToDto(u, null));
    }

    public async Task<bool> AccountExistsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accountId) || !long.TryParse(accountId, out var accountIdLong) || accountIdLong == 0)
            return false;
        return await _context.Cuenta.AnyAsync(c => c.IdCuenta == accountIdLong, cancellationToken);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var accountIdLong = long.TryParse(dto.AccountId, out var acLong) ? acLong : 0;

        var usuario = new Data.Entities.Usuario
        {
            IdCuenta = accountIdLong,
            Nombre = dto.Name,
            Apellido = dto.LastName,
            Correo = dto.Email,
            Clave = HashPassword(dto.Password),
            IdEstado = "A",
            IdPersona = dto.PersonId,
            DatosAdicionales = dto.Roles != null && dto.Roles.Length > 0
                ? JsonSerializer.Serialize(new { roles = dto.Roles })
                : JsonSerializer.Serialize(new { roles = new[] { ApiRoles.DefaultRole } })
        };

        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(usuario);
    }

    public async Task<UserDto?> UpdateUserProfileAsync(string userId, UpdateUserProfileDto dto, CancellationToken cancellationToken = default)
    {
        var userIdLong = long.TryParse(userId, out var value) ? value : 0;
        
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        if (usuario == null)
            return null;

        usuario.Nombre = dto.FirstName;
        usuario.Apellido = dto.LastName;
        usuario.Correo = dto.Email;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(usuario);
    }

    public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return false;

        var userIdLong = long.TryParse(userId, out var value) ? value : 0;
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        
        if (usuario == null)
            return false;

        // Verificar contraseña actual
        if (!VerifyPassword(dto.CurrentPassword, usuario.Clave))
            return false;

        // Actualizar contraseña
        usuario.Clave = HashPassword(dto.NewPassword);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = long.TryParse(userId, out var value) ? value : 0;
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        
        if (usuario == null)
            return false;

        usuario.IdEstado = "I"; // Inactivo

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ActivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = long.TryParse(userId, out var value) ? value : 0;
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        
        if (usuario == null)
            return false;

        usuario.IdEstado = "A"; // Activo

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Helper methods
    private UserDto MapToDto(Data.Entities.Usuario usuario, string[]? roles = null)
    {
        return new UserDto
        {
            Id = usuario.IdUsuario.ToString(),
            AccountId = usuario.IdCuenta.ToString(),
            Name = usuario.Nombre,
            LastName = usuario.Apellido,
            Email = usuario.Correo,
            Status = usuario.IdEstado,
            PartyId = usuario.IdPersona,
            PartyName = usuario.IdPersonaNavigation?.Nombre,
            Roles = roles ?? new[] { ApiRoles.DefaultRole },
            CreatedAt = DateTime.UtcNow // Usuario no tiene FechaCreacion
        };
    }

    private string HashPassword(string password)
    {
        // TODO: Cambiar a BCrypt en producción
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string? hashedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword))
            return false;

        return hashedPassword == HashPassword(password);
    }
}

