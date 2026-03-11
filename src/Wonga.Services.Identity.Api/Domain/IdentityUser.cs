namespace Wonga.Services.Identity.Domain;

public sealed class IdentityUser
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;

    public string PasswordHash { get; init; } = string.Empty;

    public string PasswordSalt { get; init; } = string.Empty;

    public DateTimeOffset CreatedUtc { get; init; }
}
