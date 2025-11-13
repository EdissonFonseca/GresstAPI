using Gresst.Application.DTOs;
using Gresst.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gresst.API.Authorization;

/// <summary>
/// Authorization attribute to check if user has specific permission on an option
/// Use: [RequirePermission("facilities", PermissionFlags.Create)]
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _optionId;
    private readonly PermissionFlags _requiredPermission;

    public RequirePermissionAttribute(string optionId, PermissionFlags requiredPermission)
    {
        _optionId = optionId;
        _requiredPermission = requiredPermission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get permission service
        var authService = context.HttpContext.RequestServices
            .GetService<IAuthorizationService>();

        if (authService == null)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        // Check permission
        var hasPermission = await authService.CurrentUserHasPermissionAsync(_optionId, _requiredPermission);

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}

