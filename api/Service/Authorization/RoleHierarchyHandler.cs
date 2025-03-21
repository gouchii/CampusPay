using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace api.Service.Authorization;

public class RoleHierarchyHandler : AuthorizationHandler<RoleHierarchyHandler.RoleRequirement>
{
    private static readonly Dictionary<string, int> RoleHierarchy = new(StringComparer.OrdinalIgnoreCase)
    {
        { "User", 1 },
        { "Merchant", 2 },
        { "Admin", 3 }
    };

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        if (!RoleHierarchy.TryGetValue(requirement.RoleName, out var requiredRoleLevel))
        {
            return Task.CompletedTask;
        }

        var userRoles = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (!userRoles.Any()) return Task.CompletedTask; 

        if (userRoles.Any(role => RoleHierarchy.TryGetValue(role, out var userRoleLevel) && userRoleLevel >= requiredRoleLevel))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    public class RoleRequirement : IAuthorizationRequirement
    {
        public string RoleName { get; }
        public RoleRequirement(string roleName) => RoleName = roleName;
    }
}