using Wonga.Services.UserProfile.Application;

namespace Wonga.Services.UserProfile.Api.Features.Users.V1;

internal static class GetCurrentUserEndpoint
{
    internal static IEndpointRouteBuilder MapGetCurrentUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/users/me", async (
            HttpContext httpContext,
            UserProfileApplicationService userProfileApplicationService,
            CancellationToken cancellationToken) =>
        {
            if (!TryGetBearerToken(httpContext, out var accessToken))
            {
                return Results.Problem(
                    title: "Unauthorized.",
                    detail: "A bearer access token is required.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            var result = await userProfileApplicationService.GetCurrentUserAsync(accessToken!, cancellationToken);
            if (!result.IsAuthorized)
            {
                return Results.Problem(
                    title: "Unauthorized.",
                    detail: "The access token is invalid or expired.",
                    statusCode: StatusCodes.Status401Unauthorized);
            }

            if (!result.ProfileExists)
            {
                return Results.Problem(
                    title: "Profile not found.",
                    detail: "No profile exists for the authenticated user.",
                    statusCode: StatusCodes.Status404NotFound);
            }

            return Results.Ok(new
            {
                firstName = result.FirstName,
                lastName = result.LastName,
                email = result.Email
            });
        });

        return endpoints;
    }

    private static bool TryGetBearerToken(HttpContext httpContext, out string? accessToken)
    {
        accessToken = null;
        if (!httpContext.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        accessToken = httpContext.Request.Headers.Authorization.ToString()["Bearer ".Length..].Trim();
        return !string.IsNullOrWhiteSpace(accessToken);
    }
}
