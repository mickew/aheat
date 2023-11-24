﻿using AHeat.Web.API.Models;
using AHeat.Web.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AHeat.Web.API.Data;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    private const string AdministratorsRole = "Administrators";
    private const string AccountsRole = "Accounts";
    private const string OperationsRole = "Operations";
    public const string DefaultAdminUserName = "admin";

    private const string DefaultPassword = "Password123!";
    public DbInitializer(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task RunAsync()
    {
        _context.Database.Migrate();

        // Create roles
        await _roleManager.CreateAsync(
            new Role
            {
                Name = AdministratorsRole,
                NormalizedName = AdministratorsRole.ToUpper(),
                Permissions = Permissions.All
            });

        await _roleManager.CreateAsync(
            new Role
            {
                Name = AccountsRole,
                NormalizedName = AccountsRole.ToUpper(),
                Permissions =
                    Permissions.ViewUsers
            });

        await _roleManager.CreateAsync(
            new Role
            {
                Name = OperationsRole,
                NormalizedName = OperationsRole.ToUpper(),
                Permissions = Permissions.ViewUsers
            });

        // Ensure admin role has all permissions
        var adminRole = await _roleManager.FindByNameAsync(AdministratorsRole);
        adminRole!.Permissions = Permissions.All;
        await _roleManager.UpdateAsync(adminRole);

        // Create default admin user
        var adminUserName = DefaultAdminUserName;
        var adminUser = new User { UserName = adminUserName, Email = adminUserName, FirstName = "Admin", LastName = "Localhost" };
        await _userManager.CreateAsync(adminUser, DefaultPassword);

        adminUser = await _userManager.FindByNameAsync(adminUserName);
        if (adminUser != null)
        {
            await _userManager.AddToRoleAsync(adminUser, AdministratorsRole);
        }

        // Create default auditor user
        var auditorUserName = "auditor";
        var auditorUser = new User { UserName = auditorUserName, Email = auditorUserName, FirstName = "Auditor", LastName = "Localhost" };
        await _userManager.CreateAsync(auditorUser, DefaultPassword);

        await _context.SaveChangesAsync();
    }
}
