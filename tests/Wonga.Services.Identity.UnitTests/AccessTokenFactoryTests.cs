using Wonga.Shared.Security;
using Wonga.Services.Identity.Application;

namespace Wonga.Services.Identity.UnitTests;

public sealed class AccessTokenFactoryTests
{
    [Fact]
    public void Create_ShouldReturnDifferentTokens()
    {
        var options = new AccessTokenOptions
        {
            Issuer = "wonga-platform",
            Audience = "wonga-platform-clients",
            SigningKey = "change-this-development-signing-key-123456",
            LifetimeMinutes = 60
        };
        var first = AccessTokenFactory.Create(Guid.NewGuid(), "first@example.com", options, DateTimeOffset.UtcNow, TimeSpan.FromMinutes(60));
        var second = AccessTokenFactory.Create(Guid.NewGuid(), "second@example.com", options, DateTimeOffset.UtcNow, TimeSpan.FromMinutes(60));

        Assert.NotEqual(first, second);
        Assert.NotEmpty(first);
        Assert.NotEmpty(second);
    }

    [Fact]
    public void Create_ShouldProduceATokenThatValidates()
    {
        var options = new AccessTokenOptions
        {
            Issuer = "wonga-platform",
            Audience = "wonga-platform-clients",
            SigningKey = "change-this-development-signing-key-123456",
            LifetimeMinutes = 60
        };
        var userId = Guid.NewGuid();
        var token = AccessTokenFactory.Create(userId, "user@example.com", options, DateTimeOffset.UtcNow, TimeSpan.FromMinutes(60));

        var validation = AccessTokenValidator.Validate(token, options, DateTimeOffset.UtcNow);

        Assert.True(validation.IsValid);
        Assert.Equal(userId, validation.UserId);
        Assert.Equal("user@example.com", validation.Email);
    }
}
