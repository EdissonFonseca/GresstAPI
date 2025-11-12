using Microsoft.Extensions.Configuration;

namespace Gresst.Infrastructure.Authentication;

/// <summary>
/// Factory to select authentication provider based on configuration
/// </summary>
public class AuthenticationServiceFactory
{
    private readonly DatabaseAuthenticationService _databaseAuth;
    private readonly ExternalAuthenticationService _externalAuth;
    private readonly IConfiguration _configuration;

    public AuthenticationServiceFactory(
        DatabaseAuthenticationService databaseAuth,
        ExternalAuthenticationService externalAuth,
        IConfiguration configuration)
    {
        _databaseAuth = databaseAuth;
        _externalAuth = externalAuth;
        _configuration = configuration;
    }

    public IAuthenticationService GetAuthenticationService()
    {
        var authMode = _configuration["Authentication:Mode"] ?? "Database";
        
        return authMode.ToLower() switch
        {
            "database" => _databaseAuth,
            "external" => _externalAuth,
            _ => _databaseAuth
        };
    }

    public IAuthenticationService GetDatabaseAuthenticationService() => _databaseAuth;
    
    public IAuthenticationService GetExternalAuthenticationService() => _externalAuth;
}

