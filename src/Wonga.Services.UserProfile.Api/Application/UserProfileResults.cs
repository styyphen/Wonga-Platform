namespace Wonga.Services.UserProfile.Application;

public sealed record CurrentUserResult(bool IsAuthorized, bool ProfileExists, string? FirstName, string? LastName, string? Email);

public sealed record TokenValidationResult(bool IsValid, Guid? UserId, string? Email, DateTimeOffset? ExpiresUtc);
