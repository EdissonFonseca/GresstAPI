using Gresst.Application.DTOs;
using Gresst.Infrastructure.Authentication.Models;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gresst.Infrastructure.Authentication;

/// <summary>
/// Authentication using database (Usuario table) with RefreshToken support
/// </summary>
public class DatabaseAuthenticationService : IAuthenticationService
{
    private readonly InfrastructureDbContext _context;
    private readonly IConfiguration _configuration;

    public DatabaseAuthenticationService(InfrastructureDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Buscar usuario en BD (por correo o por nombre)
            var usuario = await _context.Usuarios
                .Include(u => u.IdCuentaNavigation)
                .FirstOrDefaultAsync(u =>
                    (u.Correo == request.Username || u.Nombre == request.Username) &&
                    u.IdEstado == "A",
                    cancellationToken);

            if (usuario == null)
            {
                return new AuthenticationResult 
                { 
                    Success = false, 
                    Error = "Usuario no encontrado o inactivo" 
                };
            }

            // Verificar password
            if (!VerifyPassword(request.Password, usuario.Clave))
            {
                return new AuthenticationResult 
                { 
                    Success = false, 
                    Error = "Contraseña incorrecta" 
                };
            }

            // Generar JWT Access Token y Refresh Token
            var (accessToken, jwtId) = GenerateJwtToken(usuario, ClaimConstants.SubjectTypeHuman, null);
            var refreshToken = await GenerateRefreshTokenAsync(usuario.IdUsuario, jwtId, cancellationToken);
            
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes());
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays());

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = IdConversion.ToStringFromLong(usuario.IdUsuario),
                AccountId = IdConversion.ToStringFromLong(usuario.IdCuenta),
                AccountPersonId = usuario.IdCuentaNavigation?.IdPersona ?? string.Empty,
                Username = usuario.Nombre,
                Email = usuario.Correo,
                PersonId = usuario.IdPersona ?? string.Empty,
                Roles = ParseRoles(usuario.DatosAdicionales),
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResult 
            { 
                Success = false, 
                Error = $"Error de autenticación: {ex.Message}" 
            };
        }
    }

    public async Task<(bool,string)> IsUserAuthorizedForInterfaceAsync(string interfaz, string email, string token, CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.IdCuentaNavigation)
            .FirstOrDefaultAsync(u => u.Correo == email, cancellationToken);
        if (usuario != null)
        {
            var cuenta = await (
                from c in _context.CuentaInterfazs
                where c.IdCuenta == usuario.IdCuenta && c.Interfaz == interfaz && c.Token == token
                select c).FirstOrDefaultAsync(cancellationToken);
            if (cuenta != null)
            {
                var (accessToken, _) = GenerateJwtToken(usuario, ClaimConstants.SubjectTypeService, interfaz);
                return (true, accessToken);
            }
        }

        return (false, String.Empty);
    }

    public async Task<(bool, string)> IsGuestAuthorizedForInterfaceAsync(string interfaz, string token, CancellationToken cancellationToken = default)
    {
        var user = await (
            from ci in _context.CuentaInterfazs
            join c in _context.Cuenta on ci.IdCuenta equals c.IdCuenta
            join u in _context.Usuarios on c.IdCuenta equals u.IdCuenta
            where ci.Interfaz == interfaz && ci.Token == token && u.Nombre == ci.Llave
            select u).FirstOrDefaultAsync(cancellationToken);
        if (user != null)
        {
            // Cargar la navegación si no está cargada
            if (user.IdCuentaNavigation == null)
            {
                await _context.Entry(user)
                    .Reference(u => u.IdCuentaNavigation)
                    .LoadAsync(cancellationToken);
            }
            
            var (accessToken, _) = GenerateJwtToken(user, ClaimConstants.SubjectTypeService, interfaz);
            return (true, accessToken);
        }

        return (false, String.Empty);
    }

    public async Task<AuthenticationResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var accountId = principal.FindFirst("AccountId")?.Value;
            var username = principal.FindFirst(ClaimTypes.Name)?.Value;

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = token,
                UserId = userId ?? string.Empty,
                AccountId = accountId ?? string.Empty,
                Username = username
            };
        }
        catch
        {
            return new AuthenticationResult { Success = false, Error = "Token inválido o expirado" };
        }
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar el access token (aunque esté expirado, necesitamos sus claims)
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(GetJwtSecret());

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // No validar expiración para refresh
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(request.AccessToken, validationParameters, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;
            
            if (jwtToken == null)
            {
                return new AuthenticationResult { Success = false, Error = "Token inválido" };
            }

            var jwtId = jwtToken.Id;
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userIdLong))
            {
                return new AuthenticationResult { Success = false, Error = "Token inválido" };
            }

            // Buscar el refresh token en BD
            var storedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => 
                    rt.Token == request.RefreshToken && 
                    rt.JwtId == jwtId &&
                    rt.IdUsuario == userIdLong,
                    cancellationToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Success = false, Error = "Refresh token inválido" };
            }

            // Validar refresh token
            if (storedRefreshToken.IsUsed)
            {
                return new AuthenticationResult { Success = false, Error = "Refresh token ya fue usado" };
            }

            if (storedRefreshToken.IsRevoked)
            {
                return new AuthenticationResult { Success = false, Error = "Refresh token fue revocado" };
            }

            if (storedRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                return new AuthenticationResult { Success = false, Error = "Refresh token expirado" };
            }

            // Marcar el refresh token como usado
            storedRefreshToken.IsUsed = true;
            await _context.SaveChangesAsync(cancellationToken);

            // Obtener usuario actualizado
            var usuario = 
                await _context.Usuarios
                .Include(u => u.IdCuentaNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == userIdLong, cancellationToken);

            if (usuario == null || usuario.IdEstado != "A")
            {
                return new AuthenticationResult { Success = false, Error = "Usuario no encontrado o inactivo" };
            }

            // Generar nuevos tokens
            var (newAccessToken, newJwtId) = GenerateJwtToken(usuario, ClaimConstants.SubjectTypeHuman, null);
            var newRefreshToken = await GenerateRefreshTokenAsync(usuario.IdUsuario, newJwtId, cancellationToken);
            
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes());
            var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(GetRefreshTokenExpirationDays());

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = IdConversion.ToStringFromLong(usuario.IdUsuario),
                AccountId = IdConversion.ToStringFromLong(usuario.IdCuenta),
                Username = usuario.Nombre,
                Email = usuario.Correo,
                Roles = ParseRoles(usuario.DatosAdicionales),
                AccessTokenExpiresAt = accessTokenExpiresAt,
                RefreshTokenExpiresAt = refreshTokenExpiresAt
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResult { Success = false, Error = $"Error al refrescar token: {ex.Message}" };
        }
    }

    public async Task<bool> IsValidRefreshTokenAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var user = await (from u in _context.Usuarios
                    where u.Correo == email
                    select u).FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return false;

        Dictionary<string, object> settings = new Dictionary<string, object>();
        if (user.DatosAdicionales != null)
        {
            //settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(user.DatosAdicionales);
            if (settings.ContainsKey("token") && settings["token"].ToString() == token)
            {
                if (settings.ContainsKey("expiresAt"))
                {
                    var date = settings["expiresAt"]?.ToString();
                    if (date == null)
                        return true;

                    DateTime expiresAt = DateTime.Parse(date);
                    return DateTime.UtcNow < expiresAt;
                }
            }
        }

        return false;
    }

    public async Task<bool> LogoutAsync(string userId, string? refreshToken = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var userIdLong = IdConversion.ToLongFromString(userId);

            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Revocar el refresh token específico
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.IdUsuario == userIdLong, cancellationToken);

                if (token != null)
                {
                    token.IsRevoked = true;
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            else
            {
                // Revocar todos los refresh tokens del usuario
                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.IdUsuario == userIdLong && !rt.IsRevoked)
                    .ToListAsync(cancellationToken);

                foreach (var token in tokens)
                {
                    token.IsRevoked = true;
                }

                if (tokens.Any())
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GetAccountIdForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        
        return usuario != null 
            ? IdConversion.ToStringFromLong(usuario.IdCuenta) 
            : string.Empty;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email, cancellationToken);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = IdConversion.ToStringFromLong(user.IdUsuario),
            AccountId = user.IdCuenta.ToString(),
            Name = user.Nombre,
            Email = user.Correo,
            Status = user.IdEstado,
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userIdLong, cancellationToken);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = IdConversion.ToStringFromLong(user.IdUsuario),
            AccountId = user.IdCuenta.ToString(),
            Name = user.Nombre,
            Email = user.Correo,
            Status = user.IdEstado,
        };
    }

    public async Task<bool> ChangeNameAsync(string userId, string name, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userIdLong, cancellationToken);

        if (user == null)
            return false;

        user.Nombre = name;
        _context.SaveChanges();

        return true;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string password, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userIdLong, cancellationToken);

        if (user == null)
            return false;

        user.Clave = HashPassword(password);
        _context.SaveChanges();

        return true;
    }

    public async Task<bool> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == email && u.IdEstado == "A", cancellationToken);
        if (user == null)
            return true; // Always return success to avoid email enumeration

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        var expiresAt = DateTime.UtcNow.AddHours(1).ToString("O");

        var node = string.IsNullOrEmpty(user.DatosAdicionales)
            ? new JsonObject()
            : JsonNode.Parse(user.DatosAdicionales) as JsonObject ?? new JsonObject();
        node["passwordResetToken"] = token;
        node["passwordResetExpiresAt"] = expiresAt;
        user.DatosAdicionales = node.ToJsonString();
        await _context.SaveChangesAsync(cancellationToken);

        // TODO: Send email with reset link (e.g. https://app.example.com/reset-password?token=...)
        // For now the token is stored; client can use a separate flow to send the email or get the token for testing
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            return false;

        var users = await _context.Usuarios
            .Where(u => u.DatosAdicionales != null)
            .ToListAsync(cancellationToken);

        Usuario? targetUser = null;
        foreach (var u in users)
        {
            try
            {
                var node = JsonNode.Parse(u.DatosAdicionales!) as JsonObject;
                var storedToken = node?["passwordResetToken"]?.GetValue<string>();
                var expiresAtStr = node?["passwordResetExpiresAt"]?.GetValue<string>();
                if (storedToken == token && !string.IsNullOrEmpty(expiresAtStr) &&
                    DateTime.TryParse(expiresAtStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiresAt) &&
                    expiresAt > DateTime.UtcNow)
                {
                    targetUser = u;
                    break;
                }
            }
            catch
            {
                // Ignore malformed JSON
            }
        }

        if (targetUser == null)
            return false;

        targetUser.Clave = HashPassword(newPassword);
        var dataNode = JsonNode.Parse(targetUser.DatosAdicionales ?? "{}") as JsonObject ?? new JsonObject();
        dataNode.Remove("passwordResetToken");
        dataNode.Remove("passwordResetExpiresAt");
        targetUser.DatosAdicionales = dataNode.ToJsonString();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Helper methods
    private (string token, string jwtId) GenerateJwtToken(
        Usuario usuario,
        string subjectType = ClaimConstants.SubjectTypeHuman,
        string? interfaceName = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(GetJwtSecret());
        
        var jwtId = Guid.NewGuid().ToString();
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, jwtId),
            new Claim(ClaimConstants.SubjectType, subjectType),
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre ?? ""),
            new Claim(ClaimTypes.Email, usuario.Correo ?? ""),
            new Claim("AccountId", usuario.IdCuenta.ToString()),
            new Claim("AccountPersonId", usuario.IdCuentaNavigation?.IdPersona ?? ""),
            new Claim("PersonId", usuario.IdPersona ?? ""),
        };

        if (!string.IsNullOrEmpty(interfaceName))
        {
            claims.Add(new Claim(ClaimConstants.Interface, interfaceName));
        }

        // Agregar roles
        var roles = ParseRoles(usuario.DatosAdicionales);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(GetAccessTokenExpirationMinutes()),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = GetJwtIssuer(),
            Audience = GetJwtAudience()
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), jwtId);
    }

    private async Task<string?> GenerateRefreshTokenAsync(long userId, string jwtId, CancellationToken cancellationToken)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var refreshToken = Convert.ToBase64String(randomBytes);

        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userId, cancellationToken);
        if (user == null) return null;

        Dictionary<string, string>? settings = new Dictionary<string, string>();
        if (user.DatosAdicionales != null)
            settings = JsonSerializer.Deserialize<Dictionary<string, string>>(user.DatosAdicionales);

        if (settings == null)
            settings = new Dictionary<string, string>();
        settings["token"] = refreshToken;
        settings["createdAt"] = DateTime.UtcNow.ToString();
        settings["expiresAt"] = DateTime.UtcNow.AddDays(7).ToString();
        user.DatosAdicionales = JsonSerializer.Serialize(settings);
        _context.SaveChanges();

        return refreshToken;
    }

    private bool VerifyPassword(string password, string? hashedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword))
            return false;


        if (password == hashedPassword)
            return true;

        if (HashPassword(password) == hashedPassword)
            return true;

        var cryptoPassword = CryptoService.Encrypt(password);
        if (cryptoPassword == hashedPassword)
            return true;

        var cryptoPasswordOld = CryptoService.EncryptOld(password);
        if (cryptoPasswordOld == hashedPassword)
            return true;

        return false;
    }

    private string HashPassword(string password)
    {
        // Usar BCrypt o Argon2 en producción
        // Por ahora, SHA256 simple (NO usar en producción)
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private string[] ParseRoles(string? datosAdicionales)
    {
        if (string.IsNullOrEmpty(datosAdicionales))
            return new[] { "User" };

        try
        {
            var json = System.Text.Json.JsonDocument.Parse(datosAdicionales);
            if (json.RootElement.TryGetProperty("roles", out var rolesElement))
            {
                var roles = System.Text.Json.JsonSerializer.Deserialize<string[]>(rolesElement.GetRawText());
                return roles ?? new[] { "User" };
            }
            return new[] { "User" };
        }
        catch
        {
            return new[] { "User" };
        }
    }

    private string GetJwtSecret()
    {
        return _configuration["Authentication:JwtSecret"] 
            ?? "gresst-super-secret-key-change-in-production-min-32-chars";
    }

    private double GetAccessTokenExpirationMinutes()
    {
        return double.TryParse(_configuration["Authentication:AccessTokenExpirationMinutes"], out var minutes) 
            ? minutes 
            : 15; // 15 minutos por defecto
    }

    private double GetRefreshTokenExpirationDays()
    {
        return double.TryParse(_configuration["Authentication:RefreshTokenExpirationDays"], out var days) 
            ? days 
            : 7; // 7 días por defecto
    }

    private string? GetJwtIssuer()
    {
        return _configuration["Authentication:JwtIssuer"];
    }

    private string? GetJwtAudience()
    {
        return _configuration["Authentication:JwtAudience"];
    }
}
