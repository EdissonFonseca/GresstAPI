using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gresst.API.Configuration;

public sealed class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation == null) throw new ArgumentNullException(nameof(operation));
        if (context == null) throw new ArgumentNullException(nameof(context));

        var hasAuthorize =
            context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true
            || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (!hasAuthorize)
            return;

        operation.Security ??= new List<OpenApiSecurityRequirement>();

        //operation.Security.Add(new OpenApiSecurityRequirement
        //{
        //    {
        //        new OpenApiSecurityScheme
        //        {
        //            Description = "JWT",    
        //            Type = SecuritySchemeType.Http,
        //            BearerFormat = "JWT",
                        
        //        },
        //        Array.Empty<string>()
        //    }
        //});
    }
}
