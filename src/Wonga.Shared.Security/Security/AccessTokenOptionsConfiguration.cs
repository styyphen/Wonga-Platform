using Microsoft.Extensions.Configuration;

namespace Wonga.Shared.Security;

public static class AccessTokenOptionsConfiguration
{
    public static AccessTokenOptions Build(IConfiguration configuration)
    {
        var section = configuration.GetSection("Auth:AccessToken");
        var lifetimeMinutes = int.TryParse(section["LifetimeMinutes"], out var parsedLifetimeMinutes)
            ? parsedLifetimeMinutes
            : 60;

        return new AccessTokenOptions
        {
            Issuer = section["Issuer"] ?? "wonga-platform",
            Audience = section["Audience"] ?? "wonga-platform-clients",
            SigningKey = section["SigningKey"] ?? "change-this-development-signing-key-123456",
            LifetimeMinutes = lifetimeMinutes
        };
    }
}
