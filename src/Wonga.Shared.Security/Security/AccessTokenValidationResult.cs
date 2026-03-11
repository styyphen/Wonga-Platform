namespace Wonga.Shared.Security;

public sealed record AccessTokenValidationResult(
    bool IsValid,
    Guid? UserId,
    string? Email,
    DateTimeOffset? ExpiresUtc);
