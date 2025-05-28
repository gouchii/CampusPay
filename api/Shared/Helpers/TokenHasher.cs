using System.Security.Cryptography;
using System.Text;

namespace api.Shared.Helpers;

public static class TokenHasher
{
    public static string HashToken(string token, string signingKey)
    {
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new ArgumentException("Signing key must not be null or empty.", nameof(signingKey));
        }

        var hmacKey = Encoding.UTF8.GetBytes(signingKey);
        using var hmac = new HMACSHA256(hmacKey);
        return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(token)));
    }
}