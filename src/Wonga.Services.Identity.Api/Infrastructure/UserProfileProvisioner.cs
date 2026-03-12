using System.Net.Http.Json;
using Wonga.Services.Identity.Application;

namespace Wonga.Services.Identity.Infrastructure;

public sealed class UserProfileProvisioner(HttpClient httpClient) : IUserProfileProvisioner
{
    public async Task ProvisionAsync(
        Guid userId,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/internal/users/profile",
            new UpsertProfileRequest(userId, firstName, lastName, email),
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private sealed record UpsertProfileRequest(Guid UserId, string FirstName, string LastName, string Email);
}
