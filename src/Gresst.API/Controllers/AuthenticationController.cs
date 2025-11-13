using Asp.Versioning;
using Gresst.Infrastructure.Authentication;
using Gresst.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationServiceFactory _authFactory;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(AuthenticationServiceFactory authFactory, ILogger<AuthenticationController> logger)
    {
        _authFactory = authFactory;
        _logger = logger;
    }

    /// <summary>
    /// Handles a health check request and returns a successful response to indicate that the service is available.
    /// </summary>
    /// <returns>An <see cref="OkObjectResult"/> containing <see langword="true"/> to signify that the service is running.</returns>
    [HttpGet("ping")]
    [AllowAnonymous]
    public ActionResult Ping()
    {
        return Ok(true);
    }

    /// <summary>
    /// Verifies that the current request's authentication token is valid.
    /// </summary>
    /// <returns>An HTTP 200 OK response containing <see langword="true"/> if the token is valid.</returns>
    [Authorize]
    [HttpGet("validatetoken")]
    public ActionResult ValidateToken()
    {
        return Ok(true);
    }

    /// <summary>
    /// Returns information about the currently authenticated user, including user ID, username, account ID, email,
    /// roles, and authentication status.
    /// </summary>
    /// <remarks>This endpoint requires authentication and will only return user information for authenticated
    /// requests. If the user is not authenticated, the response may not include user details as expected. The returned
    /// roles array contains all roles assigned to the user.</remarks>
    /// <returns>An <see cref="ActionResult"/> containing a JSON object with details of the authenticated user. The object
    /// includes userId, username, accountId, email, roles, and isAuthenticated set to <see langword="true"/>.</returns>
    [Authorize]
    [HttpGet("isauthenticated")]
    public async Task<ActionResult> IsAuthenticated()
    {
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            _logger.LogWarning("IsAuthenticated called without an authenticated principal.");
            return Unauthorized(new { error = "Debe iniciar sesión antes de consultar este recurso." });
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("IsAuthenticated called but the token does not contain a user identifier.");
            return BadRequest(new { error = "El token no contiene el identificador del usuario (NameIdentifier)." });
        }

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
    /// Authenticates a user or guest for a specified interface and returns an authorization token if successful.
    /// </summary>
    /// <remarks>This endpoint allows anonymous access and supports both user and guest authentication for the
    /// specified interface. If the login request is invalid, a Bad Request response is returned. If authentication
    /// fails, an Unauthorized response is returned.</remarks>
    /// <param name="login">The login request containing the interface identifier, username, and token to be used for authentication. Cannot
    /// be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the authentication operation.</param>
    /// <returns>An HTTP 200 response containing the authorization token if authentication succeeds; otherwise, an HTTP 401
    /// Unauthorized or HTTP 400 Bad Request response.</returns>
    [AllowAnonymous]
    [HttpPost("authenticateforinterface")]
    public async Task<ActionResult> AuthenticateForInterface(LoginRequest login, CancellationToken cancellationToken)
    {
        if (login == null)
            return BadRequest();

        var  authService = _authFactory.GetAuthenticationService();
        var (result, token) = await authService.IsUserAuthorizedForInterfaceAsync(login.Interface, login.Username, login.Token, cancellationToken);

        if (result && token != null)
            return Ok(token);

        var (result2, token2) = await authService.IsGuestAuthorizedForInterfaceAsync(login.Interface, login.Token, cancellationToken);
        if (result2 && token2 != null)
            return Ok(token2);


        return Unauthorized();
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
    /// 
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    [HttpPost("isvalidrefreshtoken")]
    public async Task<ActionResult> IsValidRefreshToken(LoginRequest login, CancellationToken cancellationToken)
    {
        if (login == null)
            return BadRequest();

        var authService = _authFactory.GetAuthenticationService();
        var result = await authService.IsValidRefreshTokenAsync(login.Username, login.Token, cancellationToken);
        if (result)
            return Ok();

        return Unauthorized();
    }

    /// <summary>
    /// Checks whether a user exists with the specified login credentials.
    /// </summary>
    /// <param name="login">The login request containing the user's username and password. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the result of the existence check. Returns <see cref="OkResult"/> if
    /// the user exists; <see cref="UnauthorizedResult"/> if the user does not exist; or <see cref="BadRequestResult"/>
    /// if the login request is null.</returns>
    [Authorize]
    [HttpPost("existuser")]
    public async Task<ActionResult> ExistUser(LoginRequest login, CancellationToken cancellationToken)
    {
        if (login == null)
            return BadRequest();

        var authService = _authFactory.GetAuthenticationService();
        var UsuarioRepository = authService.GetUserByEmailAsync(login.Username, cancellationToken);
        if (UsuarioRepository  != null)
        {
            return Ok();
        }
        else
        {
            return Unauthorized();
        }
    }

    /// <summary>
    /// Retrieves the currently authenticated user's information based on the user ID present in the authentication
    /// token.
    /// </summary>
    /// <remarks>This method requires the caller to be authenticated. The user ID is extracted from the
    /// authentication token and must be a valid GUID. If the token does not contain a valid user ID, the method returns
    /// a Bad Request response.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult"/> containing the user's information if the user ID is valid; otherwise, a Bad
    /// Request response if the user ID is invalid.</returns>
    [Authorize]
    [HttpGet("getuser")]
    public async Task<ActionResult> GetUser(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out Guid userGuid))
            return BadRequest("Invalid user ID in token");
        var authService = _authFactory.GetAuthenticationService();
        var user = await authService.GetUserByIdAsync(userGuid, cancellationToken);
        return Ok(user);
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

    /// <summary>
    /// Retrieves the account information for the currently authenticated user.
    /// </summary>
    /// <remarks>This method requires the user to be authenticated. If the user's identity cannot be
    /// determined or is invalid, the method returns an unauthorized result.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult"/> containing the account information if the user is authenticated; otherwise, an
    /// unauthorized response.</returns>
    [Authorize]
    [HttpGet("getaccount")]
    public async Task<ActionResult> GetAccount(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            var authService = _authFactory.GetAuthenticationService();
            var account = await authService.GetAccountIdForUserAsync(userId, cancellationToken);

            return Ok(account);
        }

        return Unauthorized();
    }

    /// <summary>
    /// Attempts to change the password for the authenticated user.
    /// </summary>
    /// <remarks>This action requires authentication. The user is identified from the current security token.
    /// The password is updated for the authenticated user only.</remarks>
    /// <param name="user">The login request containing the new password to set for the user. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the password change operation.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the result of the password change operation. Returns <see
    /// cref="OkResult"/> if the password was changed successfully; <see cref="NotFoundResult"/> if the user was not
    /// found; or <see cref="BadRequestResult"/> if the request is invalid.</returns>
    [Authorize]
    [HttpPost("changepassword")]
    public async Task<ActionResult> ChangePassword(LoginRequest user, CancellationToken cancellationToken)
    {
        if (user == null)
            return BadRequest();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out Guid userGuid))
            return BadRequest("Invalid user ID in token");

        var authService = _authFactory.GetAuthenticationService();
        var changed = await authService.ChangePasswordAsync(userGuid, user.Password, cancellationToken);
        if (changed)
        {
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Attempts to change the name of the authenticated user based on the provided login request.
    /// </summary>
    /// <remarks>This action requires authentication. The user is identified from the current security token.
    /// If the user ID in the token is invalid, the request will fail.</remarks>
    /// <param name="user">The login request containing the new name for the user. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. Returns <see cref="OkResult"/> if the name
    /// was changed successfully; <see cref="NotFoundResult"/> if the user was not found; or <see
    /// cref="BadRequestResult"/> if the input is invalid.</returns>
    [Authorize]
    [HttpPost("changename")]
    public async Task<ActionResult> ChangeName(LoginRequest user, CancellationToken cancellationToken)
    {
        if (user == null)
            return BadRequest();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userId, out Guid userGuid))
            return BadRequest("Invalid user ID in token");

        var authService = _authFactory.GetAuthenticationService();
        var changed = await authService.ChangePasswordAsync(userGuid, user.Name, cancellationToken);
        if (changed)
        {
            return Ok();
        }
        else
        {
            return NotFound();
        }
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
}

public class ValidateTokenRequest
{
    public string Token { get; set; } = string.Empty;
}

public class LogoutRequest
{
    public string? RefreshToken { get; set; }
}

