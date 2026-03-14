using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Wonga.Gateway.Api.OpenApi;

internal sealed class GatewayOpenApiDocumentTransformer : IOpenApiDocumentTransformer
{
    private const string BearerSchemeName = "Bearer";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Wonga Gateway API",
            Version = document.Info.Version ?? "v1",
            Description = "Development entry point for identity and user profile APIs."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[BearerSchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT",
            Description = "Use the access token returned by POST /identity/login."
        };

        if (document.Paths.TryGetValue("/users/me", out var currentUserPath)
            && currentUserPath.Operations is not null
            && currentUserPath.Operations.TryGetValue(HttpMethod.Get, out var currentUserOperation))
        {
            currentUserOperation.Security ??= [];
            currentUserOperation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(BearerSchemeName, document, null)] = []
            });
        }

        return Task.CompletedTask;
    }
}
