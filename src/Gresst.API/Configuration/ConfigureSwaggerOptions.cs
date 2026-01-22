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
        foreach (var description in _provider.ApiVersionDescriptions)
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
