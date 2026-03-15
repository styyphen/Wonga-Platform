using Wonga.Services.UserProfile.Application;
using Wonga.Services.UserProfile.Domain;

namespace Wonga.Services.UserProfile.UnitTests;

public sealed class UserProfileApplicationServiceTests
{
    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnUnauthorizedWhenTokenIsInvalid()
    {
        var service = new UserProfileApplicationService(
            new FakeUserProfileRepository(),
            new FakeIdentityTokenValidator(new TokenValidationResult(false, null, null, null)));

        var result = await service.GetCurrentUserAsync("invalid-token", CancellationToken.None);

        Assert.False(result.IsAuthorized);
        Assert.False(result.ProfileExists);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnProfileForAValidToken()
    {
        var userId = Guid.NewGuid();
        var repository = new FakeUserProfileRepository();
        await repository.UpsertAsync(new UserProfileRecord
        {
            UserId = userId,
            FirstName = "Demo",
            LastName = "User",
            Email = "demo@example.com",
            CreatedUtc = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var service = new UserProfileApplicationService(
            repository,
            new FakeIdentityTokenValidator(new TokenValidationResult(true, userId, "demo@example.com", DateTimeOffset.UtcNow.AddMinutes(30))));

        var result = await service.GetCurrentUserAsync("valid-token", CancellationToken.None);

        Assert.True(result.IsAuthorized);
        Assert.True(result.ProfileExists);
        Assert.Equal("Demo", result.FirstName);
        Assert.Equal("User", result.LastName);
        Assert.Equal("demo@example.com", result.Email);
    }

    private sealed class FakeUserProfileRepository : IUserProfileRepository
    {
        private readonly Dictionary<Guid, UserProfileRecord> profiles = new();

        public Task UpsertAsync(UserProfileRecord profile, CancellationToken cancellationToken)
        {
            profiles[profile.UserId] = profile;
            return Task.CompletedTask;
        }

        public Task<UserProfileRecord?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            profiles.TryGetValue(userId, out var profile);
            return Task.FromResult(profile);
        }
    }

    private sealed class FakeIdentityTokenValidator(TokenValidationResult validationResult) : IIdentityTokenValidator
    {
        public Task<TokenValidationResult> ValidateAsync(string accessToken, CancellationToken cancellationToken)
        {
            return Task.FromResult(validationResult);
        }
    }
}
