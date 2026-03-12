using Wonga.Services.Identity.Application;

namespace Wonga.Services.Identity.Api.Features.Authentication.V1;

internal static class LoginEndpoint
{
    internal static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/identity/login", async (
            LoginRequest request,
            IConfiguration configuration,
            IdentityApplicationService identityApplicationService,
            CancellationToken cancellationToken) =>
        {
            var sessionLifetimeMinutes =
                configuration.GetValue<int?>("Auth:SessionLifetimeMinutes")
                ?? 60;

            var result = await identityApplicationService.LoginAsync(
                new LoginUserCommand(request.Email, request.Password),
                TimeSpan.FromMinutes(sessionLifetimeMinutes),
                cancellationToken);

            if (!result.Success)
            {
                return Results.Problem(
                    title: "Authentication failed.",
                    detail: result.Error ?? "Invalid email or password.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            return Results.Ok(new
            {
                accessToken = result.AccessToken,
                expiresUtc = result.ExpiresUtc
            });
        });

        return endpoints;
    }

    internal sealed record LoginRequest(string Email, string Password);
}
