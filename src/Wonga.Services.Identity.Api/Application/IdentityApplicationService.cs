using System.Net.Mail;
using Wonga.Shared.Security;
using Wonga.Services.Identity.Domain;

namespace Wonga.Services.Identity.Application;

public sealed class IdentityApplicationService(
    IIdentityRepository identityRepository,
    IUserProfileProvisioner userProfileProvisioner,
    AccessTokenOptions accessTokenOptions)
{
    private static readonly TimeSpan DefaultAccessTokenLifetime = TimeSpan.FromHours(1);

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

        try
        {
            await userProfileProvisioner.ProvisionAsync(
                user.Id,
                command.FirstName.Trim(),
                command.LastName.Trim(),
                normalizedEmail,
                cancellationToken);
        }
        catch
        {
            await identityRepository.DeleteUserAsync(user.Id, cancellationToken);
            throw;
        }

        return new RegisterUserResult(true, user.Id, null);
    }

    public async Task<LoginUserResult> LoginAsync(
        LoginUserCommand command,
        TimeSpan? sessionLifetime,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
        {
            return new LoginUserResult(false, null, null, "Email and password are required.");
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var user = await identityRepository.GetUserByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null || !PasswordSecurity.VerifyPassword(command.Password, user.PasswordHash, user.PasswordSalt))
        {
            return new LoginUserResult(false, null, null, "Invalid email or password.");
        }

        var utcNow = DateTimeOffset.UtcNow;
        var lifetime = sessionLifetime ?? DefaultAccessTokenLifetime;
        var accessToken = AccessTokenFactory.Create(user.Id, user.Email, accessTokenOptions, utcNow, lifetime);
        return new LoginUserResult(true, accessToken, utcNow.Add(lifetime), null);
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
