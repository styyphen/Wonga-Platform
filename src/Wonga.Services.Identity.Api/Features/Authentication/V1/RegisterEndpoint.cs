using Wonga.Services.Identity.Application;

namespace Wonga.Services.Identity.Api.Features.Authentication.V1;

internal static class RegisterEndpoint
{
    internal static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/identity/register", async (
            RegisterRequest request,
            IdentityApplicationService identityApplicationService,
            CancellationToken cancellationToken) =>
        {
            var result = await identityApplicationService.RegisterAsync(
                new RegisterUserCommand(request.FirstName, request.LastName, request.Email, request.Password),
                cancellationToken);

            if (!result.Success)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["registration"] = new[] { result.Error ?? "Registration failed." }
                });
            }

            return Results.Created(
                $"/users/{result.UserId}",
                new
                {
                    userId = result.UserId,
                    email = request.Email.Trim().ToLowerInvariant()
                });
        });

        return endpoints;
    }

    internal sealed record RegisterRequest(string FirstName, string LastName, string Email, string Password);
}
