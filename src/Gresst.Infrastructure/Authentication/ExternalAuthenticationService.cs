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
                UserId = GuidLongConverter.ToGuid(localUser.IdUsuario),
                AccountId = GuidLongConverter.ToGuid(localUser.IdCuenta),
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
                UserId = GuidLongConverter.ToGuid(localUser.IdUsuario),
                AccountId = GuidLongConverter.ToGuid(localUser.IdCuenta),
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

    public Task<bool> LogoutAsync(Guid userId, string? refreshToken = null, CancellationToken cancellationToken = default)
    {
        // Logout con proveedor externo si es necesario
        return Task.FromResult(true);
    }

    public async Task<Guid> GetAccountIdForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userIdLong = GuidLongConverter.ToLong(userId);
        var usuario = await _context.Usuarios.FindAsync(new object[] { userIdLong }, cancellationToken);
        
        return usuario != null 
            ? GuidLongConverter.ToGuid(usuario.IdCuenta) 
            : Guid.Empty;
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
            DatosAdicionales = JsonSerializer.Serialize(new { roles = new[] { "User" } })
        };

        await _context.Usuarios.AddAsync(newUser, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return newUser;
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
