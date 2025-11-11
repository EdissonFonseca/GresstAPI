namespace Gresst.Domain.Interfaces;

/// <summary>
/// Service to get current user and account information for multitenant support
/// </summary>
public interface ICurrentUserService
{
    Guid GetCurrentUserId();
    Guid GetCurrentAccountId();
    string GetCurrentUsername();
}

