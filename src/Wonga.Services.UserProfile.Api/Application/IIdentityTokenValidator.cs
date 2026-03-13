namespace Wonga.Services.UserProfile.Application;

public interface IIdentityTokenValidator
{
    Task<TokenValidationResult> ValidateAsync(string accessToken, CancellationToken cancellationToken);
}
