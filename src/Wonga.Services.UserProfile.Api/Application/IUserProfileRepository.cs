using Wonga.Services.UserProfile.Domain;

namespace Wonga.Services.UserProfile.Application;

public interface IUserProfileRepository
{
    Task UpsertAsync(UserProfileRecord profile, CancellationToken cancellationToken);

    Task<UserProfileRecord?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
