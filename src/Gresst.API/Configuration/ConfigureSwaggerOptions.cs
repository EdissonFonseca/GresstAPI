using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gresst.API.Configuration;

public sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        var descriptions = _provider.ApiVersionDescriptions;
        const string urlNote = "\n\n**Base URL:** You can call the API using either:\n- **Versioned:** `https://your-host/api/v1/...` (e.g. `/api/v1/clients`)\n- **Legacy (no version):** `https://your-host/api/...` (e.g. `/api/clients`). Requests to `/api/...` are rewritten to `/api/v1/...` for backward compatibility.";

        if (descriptions.Count == 0)
        {
            // Minimal API: no controllers, so add default v1 doc for Swagger
            options.SwaggerDoc("v1", new()
            {
                Title = "Gresst Waste Management API",
                Version = "1.0",
                Description = "Complete waste management system with traceability, inventory, and certificates (Minimal API)." + urlNote
            });
            return;
        }
        foreach (var description in descriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new()
                {
                    Title = "Gresst Waste Management API",
                    Version = description.ApiVersion.ToString(),
                    Description = (description.IsDeprecated
                        ? "⚠️ This API version has been deprecated."
                        : "Complete waste management system with traceability, inventory, and certificates.") + urlNote
                }
            );
        }
    }
}
