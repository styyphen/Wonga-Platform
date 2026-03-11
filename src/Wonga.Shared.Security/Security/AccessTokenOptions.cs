namespace Wonga.Shared.Security;

public sealed class AccessTokenOptions
{
    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SigningKey { get; init; } = string.Empty;

    public int LifetimeMinutes { get; init; } = 60;
}
