namespace Wonga.Services.Identity.Application;

public sealed record RegisterUserResult(bool Success, Guid? UserId, string? Error);

public sealed record LoginUserResult(bool Success, string? AccessToken, DateTimeOffset? ExpiresUtc, string? Error);
