using Wonga.Services.Identity.Domain;

namespace Wonga.Services.Identity.Application;

public interface IIdentityRepository
{
    Task<IdentityUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task CreateUserAsync(IdentityUser user, CancellationToken cancellationToken);

    Task DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
}
