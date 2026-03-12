namespace Wonga.Services.UserProfile.Application;

public sealed record UpsertUserProfileCommand(Guid UserId, string FirstName, string LastName, string Email);
