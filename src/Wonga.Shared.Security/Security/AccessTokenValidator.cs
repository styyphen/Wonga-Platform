using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Wonga.Shared.Security;

public static class AccessTokenValidator
{
    public static AccessTokenValidationResult Validate(
        string accessToken,
        AccessTokenOptions options,
        DateTimeOffset utcNow)
    {
        if (string.IsNullOrWhiteSpace(accessToken) || !AreOptionsValid(options))
        {
            return Invalid();
        }

        var segments = accessToken.Split('.');
        if (segments.Length != 3)
        {
            return Invalid();
        }

        var signingInput = $"{segments[0]}.{segments[1]}";
        var expectedSignature = ComputeSignature(signingInput, options.SigningKey);
        if (!FixedTimeEquals(segments[2], expectedSignature))
        {
            return Invalid();
        }

        try
        {
            using var payloadDocument = JsonDocument.Parse(Base64UrlDecode(segments[1]));
            var payload = payloadDocument.RootElement;

            var issuer = payload.GetProperty("iss").GetString();
            var audience = payload.GetProperty("aud").GetString();
            var userIdText = payload.GetProperty("sub").GetString();
            var email = payload.GetProperty("email").GetString();
            var expiresUnix = payload.GetProperty("exp").GetInt64();

            if (!string.Equals(issuer, options.Issuer, StringComparison.Ordinal) ||
                !string.Equals(audience, options.Audience, StringComparison.Ordinal) ||
                !Guid.TryParse(userIdText, out var userId) ||
                string.IsNullOrWhiteSpace(email))
            {
                return Invalid();
            }

            var expiresUtc = DateTimeOffset.FromUnixTimeSeconds(expiresUnix);
            if (expiresUtc <= utcNow)
            {
                return Invalid();
            }

            return new AccessTokenValidationResult(true, userId, email, expiresUtc);
        }
        catch
        {
            return Invalid();
        }
    }

    private static bool AreOptionsValid(AccessTokenOptions options)
    {
        return !string.IsNullOrWhiteSpace(options.Issuer) &&
               !string.IsNullOrWhiteSpace(options.Audience) &&
               !string.IsNullOrWhiteSpace(options.SigningKey) &&
               options.SigningKey.Length >= 32;
    }

    private static string ComputeSignature(string signingInput, string signingKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signingKey));
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput)));
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value
            .Replace('-', '+')
            .Replace('_', '/');

        padded = (padded.Length % 4) switch
        {
            2 => padded + "==",
            3 => padded + "=",
            _ => padded
        };

        return Convert.FromBase64String(padded);
    }

    private static AccessTokenValidationResult Invalid()
    {
        return new AccessTokenValidationResult(false, null, null, null);
    }
}
