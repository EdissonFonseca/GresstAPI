using Gresst.Domain.Interfaces;
using Gresst.Infrastructure.Common;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace Gresst.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return GuidStringConverter.ToGuid(userId);
    }

    public Guid GetCurrentAccountId()
    {
        // AccountId comes from JWT token claim (set during login)
        var accountId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("AccountId");
        return GuidStringConverter.ToGuid(accountId);
    }

    public string GetCurrentUsername()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "System";
    }

    public Guid GetCurrentPersonId()
    {
        // AccountId comes from JWT token claim (set during login)
        var personId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("PersonId");
        return GuidStringConverter.ToGuid(personId);
    }

    public Guid GetCurrentAccountPersonId()
    {
        // AccountId comes from JWT token claim (set during login)
        var accountPersonId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("AccountPersonId");
        return GuidStringConverter.ToGuid(accountPersonId);
    }
}

