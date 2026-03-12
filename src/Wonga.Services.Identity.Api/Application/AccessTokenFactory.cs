using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Wonga.Shared.Security;

namespace Wonga.Services.Identity.Application;

public static class AccessTokenFactory
{
    public static string Create(
        Guid userId,
        string email,
        AccessTokenOptions options,
        DateTimeOffset utcNow,
        TimeSpan lifetime)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ValidateOptions(options);

        var expiresUtc = utcNow.Add(lifetime);
        var header = Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            alg = "HS256",
            typ = "JWT"
        })));

        var payload = Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            sub = userId,
            email,
            iss = options.Issuer,
            aud = options.Audience,
            iat = utcNow.ToUnixTimeSeconds(),
            exp = expiresUtc.ToUnixTimeSeconds(),
            jti = Guid.NewGuid().ToString("N")
        })));

        var signature = ComputeSignature($"{header}.{payload}", options.SigningKey);
        return $"{header}.{payload}.{signature}";
    }

    private static void ValidateOptions(AccessTokenOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Audience);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.SigningKey);

        if (options.SigningKey.Length < 32)
        {
            throw new InvalidOperationException("The access token signing key must be at least 32 characters long.");
        }
    }

    private static string ComputeSignature(string signingInput, string signingKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signingKey));
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput)));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
