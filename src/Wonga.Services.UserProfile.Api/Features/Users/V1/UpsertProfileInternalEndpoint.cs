using Wonga.Services.UserProfile.Application;

namespace Wonga.Services.UserProfile.Api.Features.Users.V1;

internal static class UpsertProfileInternalEndpoint
{
    internal static IEndpointRouteBuilder MapUpsertProfileInternalEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/internal/users/profile", async (
            UpsertProfileRequest request,
            UserProfileApplicationService userProfileApplicationService,
            CancellationToken cancellationToken) =>
        {
            if (request.UserId == Guid.Empty ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["profile"] = new[] { "All profile fields are required." }
                });
            }

            await userProfileApplicationService.UpsertAsync(
                new UpsertUserProfileCommand(request.UserId, request.FirstName, request.LastName, request.Email),
                cancellationToken);

            return Results.Ok(new
            {
                message = "Profile stored.",
                request.Email
            });
        });

        return endpoints;
    }

    internal sealed record UpsertProfileRequest(Guid UserId, string FirstName, string LastName, string Email);
}
