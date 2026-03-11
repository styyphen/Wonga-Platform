using System.Net.Mail;
using Wonga.Services.Identity.Domain;

namespace Wonga.Services.Identity.Application;

public sealed class IdentityApplicationService(IIdentityRepository identityRepository)
{
    public async Task<RegisterUserResult> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (!IsValidRegistration(command, out var validationError))
        {
            return new RegisterUserResult(false, null, validationError);
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var existingUser = await identityRepository.GetUserByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            return new RegisterUserResult(false, null, "Email is already registered.");
        }

        var (hash, salt) = PasswordSecurity.HashPassword(command.Password);
        var user = new IdentityUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        await identityRepository.CreateUserAsync(user, cancellationToken);
        return new RegisterUserResult(true, user.Id, null);
    }

    private static bool IsValidRegistration(RegisterUserCommand command, out string? validationError)
    {
        if (string.IsNullOrWhiteSpace(command.FirstName) ||
            string.IsNullOrWhiteSpace(command.LastName) ||
            string.IsNullOrWhiteSpace(command.Email) ||
            string.IsNullOrWhiteSpace(command.Password))
        {
            validationError = "All registration fields are required.";
            return false;
        }

        if (command.Password.Length < 8)
        {
            validationError = "Password must be at least 8 characters long.";
            return false;
        }

        try
        {
            _ = new MailAddress(command.Email);
        }
        catch
        {
            validationError = "Email address is invalid.";
            return false;
        }

        validationError = null;
        return true;
    }
}
