using Gresst.Infrastructure.Authentication;
using Gresst.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationServiceFactory _authFactory;

    public AuthController(AuthenticationServiceFactory authFactory)
    {
        _authFactory = authFactory;
    }

    /// <summary>
    /// Login con usuario y contraseña (usa el proveedor configurado)
    /// </summary>
    /// <remarks>
    /// Ejemplo:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///         "username": "admin",
    ///         "password": "password123"
    ///     }
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthenticationResult>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var authService = _authFactory.GetAuthenticationService();
        var result = await authService.LoginAsync(request, cancellationToken);

        if (!result.Success)
            return Unauthorized(new { error = result.Error });

        return Ok(result);
    }

    /// <summary>
    /// Login usando autenticación de base de datos específicamente
    /// </summary>
    [HttpPost("login/database")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), 200)]
    public async Task<ActionResult<AuthenticationResult>> LoginDatabase([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authService = _authFactory.GetDatabaseAuthenticationService();
        var result = await authService.LoginAsync(request, cancellationToken);

        if (!result.Success)
            return Unauthorized(new { error = result.Error });

        return Ok(result);
    }

    /// <summary>
    /// Login usando proveedor externo (Auth0, Azure AD, etc.)
    /// </summary>
    [HttpPost("login/external")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), 200)]
    public async Task<ActionResult<AuthenticationResult>> LoginExternal([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authService = _authFactory.GetExternalAuthenticationService();
        var result = await authService.LoginAsync(request, cancellationToken);

        if (!result.Success)
            return Unauthorized(new { error = result.Error });

        return Ok(result);
    }

    /// <summary>
    /// Validar token JWT
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(AuthenticationResult), 200)]
    public async Task<ActionResult<AuthenticationResult>> ValidateToken([FromBody] ValidateTokenRequest request, CancellationToken cancellationToken)
    {
        var authService = _authFactory.GetAuthenticationService();
        var result = await authService.ValidateTokenAsync(request.Token, cancellationToken);

        if (!result.Success)
            return Unauthorized(new { error = result.Error });

        return Ok(result);
    }

    /// <summary>
    /// Refresh AccessToken using RefreshToken
    /// </summary>
    /// <remarks>
    /// When your AccessToken expires, use this endpoint to get a new one using your RefreshToken
    /// 
    /// Example:
    /// 
    ///     POST /api/auth/refresh
    ///     {
    ///         "accessToken": "expired-jwt-token",
    ///         "refreshToken": "valid-refresh-token"
    ///     }
    /// </remarks>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthenticationResult>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var authService = _authFactory.GetAuthenticationService();
        var result = await authService.RefreshTokenAsync(request, cancellationToken);

        if (!result.Success)
            return Unauthorized(new { error = result.Error });

        return Ok(result);
    }

    /// <summary>
    /// Logout (invalidar sesión y revocar refresh token)
    /// </summary>
    /// <remarks>
    /// Optionally send the refreshToken in the body to revoke it
    /// 
    ///     POST /api/auth/logout
    ///     {
    ///         "refreshToken": "token-to-revoke"
    ///     }
    /// </remarks>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(200)]
    public async Task<ActionResult> Logout([FromBody] LogoutRequest? request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            var authService = _authFactory.GetAuthenticationService();
            await authService.LogoutAsync(userId, request?.RefreshToken, cancellationToken);
        }

        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Obtener información del usuario actual autenticado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(200)]
    public ActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var accountId = User.FindFirst("AccountId")?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToArray();

        return Ok(new
        {
            userId,
            username,
            accountId,
            email,
            roles,
            isAuthenticated = true
        });
    }
}

public class ValidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
}

public class LogoutRequest
{
    public string? RefreshToken { get; set; }
}

