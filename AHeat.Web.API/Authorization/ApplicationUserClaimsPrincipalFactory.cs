﻿using AHeat.Web.API.Models;
using AHeat.Web.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AHeat.Web.API.Authorization;

public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, Role>
{
    public ApplicationUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
    : base(userManager, roleManager, optionsAccessor)
    { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var userRoleNames = await UserManager.GetRolesAsync(user) ?? Array.Empty<string>();

        var userRoles = await RoleManager.Roles.Where(r =>
            userRoleNames.Contains(r.Name!)).ToListAsync();

        var userPermissions = Permissions.None;

        foreach (var role in userRoles)
            userPermissions |= role.Permissions;

        var permissionsValue = (int)userPermissions;

        identity.AddClaim(
            new Claim(CustomClaimTypes.Permissions, permissionsValue.ToString()));
        identity.AddClaim(
            new Claim(CustomClaimTypes.FirstName, user.FirstName));
        identity.AddClaim(
            new Claim(CustomClaimTypes.LastName, user.LastName));

        return identity;
    }
}
