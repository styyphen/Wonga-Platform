namespace Wonga.Services.Identity.Application;

public sealed record RegisterUserCommand(string FirstName, string LastName, string Email, string Password);
