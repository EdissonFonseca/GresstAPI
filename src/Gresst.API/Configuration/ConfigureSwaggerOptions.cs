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
        if (descriptions.Count == 0)
        {
            // Minimal API: no controllers, so add default v1 doc for Swagger
            options.SwaggerDoc("v1", new()
            {
                Title = "Gresst Waste Management API",
                Version = "1.0",
                Description = "Complete waste management system with traceability, inventory, and certificates (Minimal API)"
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
                    Description = description.IsDeprecated
                        ? "⚠️ This API version has been deprecated."
                        : "Complete waste management system with traceability, inventory, and certificates"
                }
            );
        }
    }
}
