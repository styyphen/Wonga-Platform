using Wonga.Gateway.Api.Features;

namespace Wonga.Gateway.Api.Features.Users.V1;

internal static class GetCurrentUserEndpoint
{
    internal static IEndpointRouteBuilder MapGetCurrentUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/users/me", async (
            HttpContext httpContext,
            IHttpClientFactory httpClientFactory,
            CancellationToken cancellationToken) =>
        {
            return await httpClientFactory.ProxyAuthorizedGetAsync(
                "user-profile-service",
                "/users/me",
                httpContext,
                "user profile service",
                cancellationToken);
        })
        .WithName("GatewayGetCurrentUser")
        .WithTags("Users")
        .WithSummary("Get the current authenticated user profile.")
        .WithDescription("Forwards the authenticated profile request to the user profile service.");

        return endpoints;
    }
}
