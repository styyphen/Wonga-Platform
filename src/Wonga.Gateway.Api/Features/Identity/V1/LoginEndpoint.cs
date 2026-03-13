using Wonga.Gateway.Api.Features;

namespace Wonga.Gateway.Api.Features.Identity.V1;

internal static class LoginEndpoint
{
    internal static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/identity/login", async (
            LoginRequest request,
            IHttpClientFactory httpClientFactory,
            CancellationToken cancellationToken) =>
        {
            return await httpClientFactory.ProxyPostAsJsonAsync(
                "identity-service",
                "/identity/login",
                request,
                "identity service",
                cancellationToken);
        })
        .WithName("GatewayLogin")
        .WithTags("Identity")
        .WithSummary("Authenticate a user and return an access token.")
        .WithDescription("Forwards login requests to the identity service through the gateway.");

        return endpoints;
    }

    internal sealed record LoginRequest(string Email, string Password);
}
