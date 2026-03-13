using Wonga.Gateway.Api.Features;

namespace Wonga.Gateway.Api.Features.Identity.V1;

internal static class RegisterEndpoint
{
    internal static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/identity/register", async (
            RegisterRequest request,
            IHttpClientFactory httpClientFactory,
            CancellationToken cancellationToken) =>
        {
            return await httpClientFactory.ProxyPostAsJsonAsync(
                "identity-service",
                "/identity/register",
                request,
                "identity service",
                cancellationToken);
        })
        .WithName("GatewayRegister")
        .WithTags("Identity")
        .WithSummary("Register a new user.")
        .WithDescription("Forwards the registration request to the identity service through the gateway.");

        return endpoints;
    }

    internal sealed record RegisterRequest(string FirstName, string LastName, string Email, string Password);
}
