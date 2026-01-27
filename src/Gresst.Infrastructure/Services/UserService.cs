using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = GuidLongConverter.ToLong(userId);
        
        var usuario = await _context.Usuarios
            .Include(u => u.IdPersonaNavigation)
            .FirstOrDefaultAsync(u => u.IdUsuario == userIdLong, cancellationToken);

        if (usuario == null)
            return null;

        return MapToDto(usuario);
    }

    public async Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == Guid.Empty)
            return null;

        return await GetUserByIdAsync(currentUserId, cancellationToken);
    }

    public async Task<IEnumerable<UserDto>> GetUsersByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var accountIdLong = GuidLongConverter.ToLong(accountId);
        
        var usuarios = await _context.Usuarios
            .Include(u => u.IdPersonaNavigation)
            .Where(u => u.IdCuenta == accountIdLong)
            .ToListAsync(cancellationToken);

        return usuarios.Select(MapToDto);
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
            IdPersona = dto.PersonId.HasValue ? GuidStringConverter.ToString(dto.PersonId.Value) : null,
            DatosAdicionales = dto.Roles != null && dto.Roles.Length > 0
                ? JsonSerializer.Serialize(new { roles = dto.Roles })
                : JsonSerializer.Serialize(new { roles = new[] { "User" } })
        };

        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(usuario);
    }

    public async Task<UserDto?> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto dto, CancellationToken cancellationToken = default)
    {
        var userIdLong = GuidLongConverter.ToLong(userId);
        
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        if (usuario == null)
            return null;

        usuario.Nombre = dto.Name;
        usuario.Apellido = dto.LastName;
        usuario.Correo = dto.Email;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(usuario);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return false;

        var userIdLong = GuidLongConverter.ToLong(userId);
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

    public async Task<bool> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = GuidLongConverter.ToLong(userId);
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        
        if (usuario == null)
            return false;

        usuario.IdEstado = "I"; // Inactivo

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ActivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = GuidLongConverter.ToLong(userId);
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        
        if (usuario == null)
            return false;

        usuario.IdEstado = "A"; // Activo

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Helper methods
    private UserDto MapToDto(Data.Entities.Usuario usuario)
    {
        string[]? roles = null;
        if (!string.IsNullOrEmpty(usuario.DatosAdicionales))
        {
            try
            {
                var json = JsonDocument.Parse(usuario.DatosAdicionales);
                if (json.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    roles = JsonSerializer.Deserialize<string[]>(rolesElement.GetRawText());
                }
            }
            catch { /* Ignorar errores de parseo */ }
        }

        return new UserDto
        {
            Id = GuidLongConverter.ToGuid(usuario.IdUsuario).ToString(),
            AccountId = usuario.IdCuenta.ToString(),
            Name = usuario.Nombre,
            LastName = usuario.Apellido,
            Email = usuario.Correo,
            Status = usuario.IdEstado,
            PersonId = !string.IsNullOrEmpty(usuario.IdPersona) 
                ? GuidStringConverter.ToGuid(usuario.IdPersona) 
                : null,
            PersonName = usuario.IdPersonaNavigation?.Nombre,
            Roles = roles ?? new[] { "User" },
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

