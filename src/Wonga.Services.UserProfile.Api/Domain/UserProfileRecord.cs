namespace Wonga.Services.UserProfile.Domain;

public sealed class UserProfileRecord
{
    public Guid UserId { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public DateTimeOffset CreatedUtc { get; init; }
}
