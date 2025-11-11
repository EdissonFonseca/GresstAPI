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

    public Guid GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
    }

    public Guid GetCurrentAccountId()
    {
        // For demo purposes, using a claim or header
        var accountId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("AccountId");
        
        if (string.IsNullOrEmpty(accountId))
        {
            // Try from header
            accountId = _httpContextAccessor.HttpContext?.Request.Headers["X-Account-Id"].FirstOrDefault();
        }

        return Guid.TryParse(accountId, out var id) ? id : Guid.Empty;
    }

    public string GetCurrentUsername()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "System";
    }
}

