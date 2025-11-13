using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gresst.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Obtener perfil completo del usuario actual
    /// </summary>
    /// <remarks>
    /// Devuelve TODOS los datos del usuario desde la BD:
    /// - Nombre, apellido, email, estado
    /// - Persona asociada (si tiene)
    /// - Roles
    /// - Fechas de creación y último acceso
    /// </remarks>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var user = await _userService.GetCurrentUserAsync(cancellationToken);
        
        if (user == null)
            return NotFound(new { error = "Usuario no encontrado" });

        return Ok(user);
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        
        if (user == null)
            return NotFound(new { error = "Usuario no encontrado" });

        return Ok(user);
    }

    /// <summary>
    /// Obtener todos los usuarios de una cuenta
    /// </summary>
    [HttpGet("account/{accountId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByAccount(Guid accountId, CancellationToken cancellationToken)
    {
        var users = await _userService.GetUsersByAccountAsync(accountId, cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Crear nuevo usuario
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userService.CreateUserAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Actualizar perfil del usuario actual
    /// </summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserDto>> UpdateMyProfile([FromBody] UpdateUserProfileDto dto, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await _userService.UpdateUserProfileAsync(userId, dto, cancellationToken);
        
        if (user == null)
            return NotFound(new { error = "Usuario no encontrado" });

        return Ok(user);
    }

    /// <summary>
    /// Actualizar perfil de otro usuario (solo admin)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserProfileDto dto, CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateUserProfileAsync(id, dto, cancellationToken);
        
        if (user == null)
            return NotFound(new { error = "Usuario no encontrado" });

        return Ok(user);
    }

    /// <summary>
    /// Cambiar contraseña del usuario actual
    /// </summary>
    [HttpPost("me/change-password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> ChangeMyPassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        if (dto.NewPassword != dto.ConfirmPassword)
            return BadRequest(new { error = "Las contraseñas no coinciden" });

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var success = await _userService.ChangePasswordAsync(userId, dto, cancellationToken);
        
        if (!success)
            return BadRequest(new { error = "Contraseña actual incorrecta" });

        return Ok(new { message = "Contraseña actualizada exitosamente" });
    }

    /// <summary>
    /// Desactivar usuario (solo admin)
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken)
    {
        var success = await _userService.DeactivateUserAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { error = "Usuario no encontrado" });

        return Ok(new { message = "Usuario desactivado" });
    }

    /// <summary>
    /// Activar usuario (solo admin)
    /// </summary>
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> ActivateUser(Guid id, CancellationToken cancellationToken)
    {
        var success = await _userService.ActivateUserAsync(id, cancellationToken);
        
        if (!success)
            return NotFound(new { error = "Usuario no encontrado" });

        return Ok(new { message = "Usuario activado" });
    }
}

