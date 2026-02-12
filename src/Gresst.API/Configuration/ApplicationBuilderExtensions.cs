using Asp.Versioning.ApiExplorer;
using Gresst.API.Endpoints;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Rewrite;
using Serilog;
using System.Runtime.InteropServices;

namespace Gresst.API.Configuration;

public static class ApplicationBuilderExtensions
{
    private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public static IApplicationBuilder UseGresstPipeline(this WebApplication app)
    {
        var corsOrigins = app.Configuration.GetValue<string>("Cors:AllowedOrigins") ?? string.Empty;
        Log.Information("[Startup] CORS AllowedOrigins: {Origins}", string.IsNullOrEmpty(corsOrigins) ? "(none)" : corsOrigins);

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseExceptionHandler(ConfigureExceptionHandler);

        if (app.Environment.IsProduction() || IsWindows)
            app.UseHttpsRedirection();

        var rewriteOptions = new RewriteOptions()
            .AddRewrite(@"^api/(?!v1/)(.*)$", "/api/v1/$1", skipRemainingRules: true);
        app.UseRewriter(rewriteOptions);
        app.UseCors("AllowAll");

        if (app.Configuration.GetValue<bool>("Swagger:Enabled", true))
            UseSwaggerUi(app);

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHealthChecks("/health");
        app.MapApiEndpoints();

        return app;
    }

    private static void ConfigureExceptionHandler(IApplicationBuilder errorApp)
    {
        errorApp.Run(async context =>
        {
            var feature = context.Features.Get<IExceptionHandlerPathFeature>();
            if (feature?.Error != null)
            {
                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("GlobalException");
                logger.LogError(feature.Error, "Unhandled exception: {Path}", context.Request.Path);
            }
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        });
    }

    private static void UseSwaggerUi(WebApplication app)
    {
        app.UseSwagger();
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseSwaggerUI(options =>
        {
            var descriptions = provider.ApiVersionDescriptions;
            if (descriptions.Count == 0)
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gresst API v1");
            else
            {
                foreach (var d in descriptions)
                    options.SwaggerEndpoint($"/swagger/{d.GroupName}/swagger.json", $"Gresst API {d.GroupName.ToUpperInvariant()}");
            }
            options.RoutePrefix = string.Empty;
        });
    }
}
