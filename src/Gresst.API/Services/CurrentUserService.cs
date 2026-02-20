using Gresst.Domain.Interfaces;
using System.Security.Claims;

namespace Gresst.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId ?? string.Empty;
    }

    public string GetCurrentAccountId()
    {
        // AccountId comes from JWT token claim (set during login) - keep as string for BaseEntity.AccountId
        var accountId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("AccountId");
        return accountId ?? string.Empty;
    }

    public string GetCurrentUsername()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "System";
    }

    public string GetCurrentPersonId()
    {
        var personId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("PersonId");
        return personId ?? string.Empty;
    }

    public string GetCurrentAccountPersonId()
    {
        var accountPersonId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("AccountPersonId");
        return accountPersonId ?? string.Empty;
    }
}

