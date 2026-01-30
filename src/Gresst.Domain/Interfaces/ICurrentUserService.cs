namespace Gresst.Domain.Interfaces;

/// <summary>
/// Service to get current user and account information for multitenant support
/// </summary>
public interface ICurrentUserService
{
    string GetCurrentUserId();
    string GetCurrentAccountId();
    string GetCurrentUsername();
    string GetCurrentPersonId();
    string GetCurrentAccountPersonId();
}

