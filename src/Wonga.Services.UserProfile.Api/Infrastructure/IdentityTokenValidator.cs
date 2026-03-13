using Wonga.Shared.Security;
using Wonga.Services.UserProfile.Application;

namespace Wonga.Services.UserProfile.Infrastructure;

public sealed class IdentityTokenValidator(AccessTokenOptions accessTokenOptions) : IIdentityTokenValidator
{
    public async Task<TokenValidationResult> ValidateAsync(string accessToken, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        var payload = AccessTokenValidator.Validate(accessToken, accessTokenOptions, DateTimeOffset.UtcNow);
        return new TokenValidationResult(payload.IsValid, payload.UserId, payload.Email, payload.ExpiresUtc);
    }
}
