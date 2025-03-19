using System.Security.Claims;

namespace api.Extensions;

public static class ClaimsExtensions
{
    public static string? GetUsername(this ClaimsPrincipal user)
    {
        return user.Claims.SingleOrDefault(x => x.Type.Equals(ClaimTypes.GivenName))?.Value;
    }

    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.Claims.SingleOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
    }

    public static string? GetRole(this ClaimsPrincipal user)
    {
        return user.Claims.SingleOrDefault(x => x.Type.Equals(ClaimTypes.Role))?.Value;
    }
}