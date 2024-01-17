using AHeat.Web.API.Models;
using AHeat.Web.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AHeat.Web.API.Data;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;

    private const string AdministratorsRole = "Administrators";
    public const string DefaultAdminUserName = "admin";

    private const string DefaultPassword = "Password123!";
    public DbInitializer(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        _context.Database.Migrate();

        Role adminRole = new Role()
        {
            Name = AdministratorsRole,
            NormalizedName = AdministratorsRole.ToUpper(),
            Permissions = Permissions.All
        };

        if (!await _context.Roles.AnyAsync())
        {
            await _context.Roles.AddAsync(adminRole);
        }

        // Create default admin user
        var adminUserName = DefaultAdminUserName;
        var adminUser = new User()
        {
            UserName = adminUserName,
            Email = "admin@localhost.local",
            FirstName = "Admin",
            LastName = "Localhost",
            NormalizedUserName = adminUserName.ToUpper(),
            NormalizedEmail = "admin@localhost.local"
        };
        
        PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
        var pw = passwordHasher.HashPassword(adminUser, DefaultPassword);
        adminUser.PasswordHash = pw;
        if (!await _context.Users.AnyAsync())
        {
            await _context.Users.AddAsync(adminUser);
            await _context.AddAsync(new IdentityUserRole<string>() { RoleId = adminRole.Id, UserId = adminUser.Id });
        }
        await _context.SaveChangesAsync();
    }
}
