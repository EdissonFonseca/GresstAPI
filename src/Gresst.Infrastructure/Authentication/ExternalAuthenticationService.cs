using Gresst.Application.Constants;
using Gresst.Application.DTOs;
using Gresst.Infrastructure.Authentication.Models;
using Gresst.Infrastructure.Common;
using Gresst.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace Gresst.Infrastructure.Authentication;

/// <summary>
/// Authentication using external provider (Auth0, Azure AD, Firebase, etc.)
/// </summary>
public class ExternalAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly InfrastructureDbContext _context;
    private readonly IConfiguration _configuration;

    public ExternalAuthenticationService(
        HttpClient httpClient,
        InfrastructureDbContext context,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthenticationResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var providerUrl = _configuration["Authentication:ExternalProvider:Url"];
            var clientId = _configuration["Authentication:ExternalProvider:ClientId"];
            var clientSecret = _configuration["Authentication:ExternalProvider:ClientSecret"];

            // Llamar al proveedor externo (ejemplo genérico)
            var payload = new
            {
                username = request.Username,
                password = request.Password,
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "password"
            };

            var response = await _httpClient.PostAsJsonAsync($"{providerUrl}/oauth/token", payload, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new AuthenticationResult 
                { 
                    Success = false, 
                    Error = "Credenciales inválidas en proveedor externo" 
                };
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<ExternalTokenResponse>(cancellationToken: cancellationToken);

            if (tokenResponse == null)
            {
                return new AuthenticationResult 
                { 
                    Success = false, 
                    Error = "Respuesta inválida del proveedor" 
                };
            }

            // Obtener información del usuario del proveedor
            var userInfo = await GetExternalUserInfoAsync(tokenResponse.AccessToken, providerUrl, cancellationToken);

            // Sincronizar con base de datos local
            var localUser = await SyncUserWithDatabaseAsync(userInfo, cancellationToken);

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken, // External provider refresh token
                UserId = IdConversion.ToStringFromLong(localUser.IdUsuario),
                AccountId = IdConversion.ToStringFromLong(localUser.IdCuenta),
                Username = localUser.Nombre,
                Email = localUser.Correo,
                AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResult 
            { 
                Success = false, 
                Error = $"Error con proveedor externo: {ex.Message}" 
            };
        }
    }
    
    public async Task<(bool, string)> IsUserAuthorizedForInterfaceAsync(string interfaz, string email, string token, CancellationToken cancellationToken = default)
    {
        return (false, String.Empty);
    }
    
    public async Task<(bool, string)> IsGuestAuthorizedForInterfaceAsync(string interfaz, string token, CancellationToken cancellationToken = default)
    {
        return (false, String.Empty);
    }
    
    public async Task<AuthenticationResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var providerUrl = _configuration["Authentication:ExternalProvider:Url"];
            
            // Validar token con el proveedor externo
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{providerUrl}/userinfo", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new AuthenticationResult { Success = false, Error = "Token inválido" };
            }

            var userInfo = await response.Content.ReadFromJsonAsync<ExternalUserInfo>(cancellationToken: cancellationToken);

            if (userInfo == null)
            {
                return new AuthenticationResult { Success = false, Error = "No se pudo obtener información del usuario" };
            }

            // Buscar usuario local
            var localUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == userInfo.Email, cancellationToken);

            if (localUser == null)
            {
                return new AuthenticationResult { Success = false, Error = "Usuario no sincronizado" };
            }

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = token,
                UserId = IdConversion.ToStringFromLong(localUser.IdUsuario),
                AccountId = IdConversion.ToStringFromLong(localUser.IdCuenta),
                Username = localUser.Nombre,
                Email = localUser.Correo
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResult 
            { 
                Success = false, 
                Error = $"Error validando token: {ex.Message}" 
            };
        }
    }
    
    public Task<AuthenticationResult> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        // Implementar con el proveedor externo si es necesario
        // Por ahora, external provider no soporta refresh token
        throw new NotImplementedException("Refresh token debe implementarse según el proveedor externo");
    }

    public async Task<bool> IsValidRefreshTokenAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        return false;
    }

    public Task<bool> LogoutAsync(string userId, string? refreshToken = null, CancellationToken cancellationToken = default)
    {
        // Logout con proveedor externo si es necesario
        return Task.FromResult(true);
    }

    public async Task<string> GetAccountIdForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = IdConversion.ToLongFromString(userId);
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        
        return usuario != null 
            ? IdConversion.ToStringFromLong(usuario.IdCuenta) 
            : string.Empty;
    }

    // Helper methods
    private async Task<ExternalUserInfo> GetExternalUserInfoAsync(string accessToken, string providerUrl, CancellationToken cancellationToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync($"{providerUrl}/userinfo", cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ExternalUserInfo>(cancellationToken: cancellationToken) 
            ?? new ExternalUserInfo();
    }

    private async Task<Data.Entities.Usuario> SyncUserWithDatabaseAsync(ExternalUserInfo userInfo, CancellationToken cancellationToken)
    {
        // Buscar usuario existente
        var existingUser = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == userInfo.Email, cancellationToken);

        if (existingUser != null)
        {
            // Ya existe, retornar
            return existingUser;
        }

        // Crear nuevo usuario (requiere una cuenta por defecto)
        var defaultAccount = await _context.Cuenta.FirstOrDefaultAsync(cancellationToken);
        if (defaultAccount == null)
        {
            throw new InvalidOperationException("No default account found for external user sync");
        }

        var newUser = new Data.Entities.Usuario
        {
            Correo = userInfo.Email,
            Nombre = userInfo.Name,
            IdEstado = "A",
            IdCuenta = defaultAccount.IdCuenta,
            Clave = Guid.NewGuid().ToString(), // Password placeholder (no se usa)
            DatosAdicionales = JsonSerializer.Serialize(new { roles = new[] { ApiRoles.DefaultRole } })
        };

        await _context.Usuarios.AddAsync(newUser, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return newUser;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return null;
    }
    
    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return null;
    }

    public async Task<bool> ChangeNameAsync(string userId, string name, CancellationToken cancellationToken = default)
    {
        return false;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string password, CancellationToken cancellationToken = default)
    {
        return false;
    }

    public Task<bool> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true); // Success to avoid enumeration; no-op for external provider
    }

    public Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false); // Not supported for external provider
    }

    public Task<ServiceTokenResult?> IssueServiceTokenAsync(ServiceTokenRequest request, CancellationToken cancellationToken = default)
    {
        // Service tokens (client credentials) are issued only when using database authentication.
        return Task.FromResult<ServiceTokenResult?>(null);
    }
}

// DTOs para proveedor externo
internal class ExternalTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string? RefreshToken { get; set; }
}

internal class ExternalUserInfo
{
    public string Sub { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
}
