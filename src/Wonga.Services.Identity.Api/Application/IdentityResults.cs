namespace Wonga.Services.Identity.Application;

public sealed record RegisterUserResult(bool Success, Guid? UserId, string? Error);
