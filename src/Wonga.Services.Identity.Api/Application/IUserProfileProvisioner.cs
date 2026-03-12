namespace Wonga.Services.Identity.Application;

public interface IUserProfileProvisioner
{
    Task ProvisionAsync(Guid userId, string firstName, string lastName, string email, CancellationToken cancellationToken);
}
