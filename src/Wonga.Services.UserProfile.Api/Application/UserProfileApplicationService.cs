using Wonga.Services.UserProfile.Domain;

namespace Wonga.Services.UserProfile.Application;

public sealed class UserProfileApplicationService(
    IUserProfileRepository userProfileRepository,
    IIdentityTokenValidator identityTokenValidator)
{
    public async Task UpsertAsync(UpsertUserProfileCommand command, CancellationToken cancellationToken)
    {
        var profile = new UserProfileRecord
        {
            UserId = command.UserId,
            FirstName = command.FirstName.Trim(),
            LastName = command.LastName.Trim(),
            Email = command.Email.Trim().ToLowerInvariant(),
            CreatedUtc = DateTimeOffset.UtcNow
        };

        await userProfileRepository.UpsertAsync(profile, cancellationToken);
    }

    public async Task<CurrentUserResult> GetCurrentUserAsync(string accessToken, CancellationToken cancellationToken)
    {
        var validationResult = await identityTokenValidator.ValidateAsync(accessToken, cancellationToken);
        if (!validationResult.IsValid || validationResult.UserId is null)
        {
            return new CurrentUserResult(false, false, null, null, null);
        }

        var profile = await userProfileRepository.GetByUserIdAsync(validationResult.UserId.Value, cancellationToken);
        if (profile is null)
        {
            return new CurrentUserResult(true, false, null, null, null);
        }

        return new CurrentUserResult(true, true, profile.FirstName, profile.LastName, profile.Email);
    }
}
