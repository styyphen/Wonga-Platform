using Wonga.Services.UserProfile.Domain;

namespace Wonga.Services.UserProfile.Application;

public sealed class UserProfileApplicationService(IUserProfileRepository userProfileRepository)
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
}
