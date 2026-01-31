using Gresst.Application.Constants;
using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Gresst.Infrastructure.Data;
using Gresst.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gresst.Infrastructure.Services;

/// <summary>
/// Creates a new account with its first administrator user (and optionally a minimal person for legal rep).
/// </summary>
public class AccountRegistrationService : IAccountRegistrationService
{
    private readonly InfrastructureDbContext _context;
    private readonly IUserService _userService;

    public AccountRegistrationService(InfrastructureDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<RegisterAccountResultDto?> RegisterAccountAsync(RegisterAccountRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.AdminEmail) || string.IsNullOrWhiteSpace(request.AdminPassword))
            return null;

        var existingUser = await _userService.GetUserByEmailAsync(request.AdminEmail, cancellationToken);
        if (existingUser != null)
            return null;

        string personId;
        Persona? personaToUpdate = null;

        if (!string.IsNullOrWhiteSpace(request.PersonId))
        {
            var existingPerson = await _context.Personas.FindAsync(new object[] { request.PersonId }, cancellationToken);
            if (existingPerson == null)
                return null;
            personId = request.PersonId;
        }
        else
        {
            var newPerson = new Persona
            {
                IdPersona = Guid.NewGuid().ToString("N")[..40],
                Nombre = request.AdminName + (string.IsNullOrWhiteSpace(request.AdminLastName) ? "" : " " + request.AdminLastName),
                Correo = request.AdminEmail,
                Activo = true,
                IdUsuarioCreacion = 0,
                FechaCreacion = DateTime.UtcNow
            };
            _context.Personas.Add(newPerson);
            await _context.SaveChangesAsync(cancellationToken);
            personId = newPerson.IdPersona;
            personaToUpdate = newPerson;
        }

        var cuenta = new Cuentum
        {
            IdUsuario = 0,
            IdPersona = personId,
            Nombre = string.IsNullOrWhiteSpace(request.AccountName) ? request.AdminEmail : request.AccountName,
            IdRol = "N",
            IdEstado = "A",
            Ajustes = null,
            PermisosPorSede = false,
            IdUsuarioCreacion = 0,
            FechaCreacion = DateTime.UtcNow
        };
        _context.Cuenta.Add(cuenta);
        await _context.SaveChangesAsync(cancellationToken);

        var accountId = cuenta.IdCuenta.ToString();

        var createUserDto = new CreateUserDto
        {
            AccountId = accountId,
            Name = request.AdminName,
            LastName = request.AdminLastName,
            Email = request.AdminEmail,
            Password = request.AdminPassword,
            PersonId = personId,
            Roles = new[] { ApiRoles.AccountAdminRole }
        };

        var adminUser = await _userService.CreateUserAsync(createUserDto, cancellationToken);
        var adminUserIdLong = long.Parse(adminUser.Id);

        cuenta.IdUsuario = adminUserIdLong;
        cuenta.IdUsuarioCreacion = adminUserIdLong;
        if (personaToUpdate != null)
        {
            personaToUpdate.IdCuenta = cuenta.IdCuenta;
            personaToUpdate.IdUsuarioCreacion = adminUserIdLong;
        }
        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterAccountResultDto
        {
            AccountId = accountId,
            AdminUser = adminUser
        };
    }
}
