
using System.IdentityModel.Tokens.Jwt;
using AHeat.Web.API.Authorization;
using AHeat.Web.API.Data;
using AHeat.Web.API.Models;
using AHeat.Web.Shared.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AHeat.Application.Interfaces;
using AHeat.Application.Services;
using AHeat.Web.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using AHeat.Web.API.Hubs;
using Microsoft.AspNetCore.HttpOverrides;

namespace AHeat.Web.API;

public class Program
{
    private const string SeedArgs = "/seed";

    public static async Task Main(string[] args)
    {
        var applyDbMigrationWithDataSeedFromProgramArguments = args.Any(x => x == SeedArgs);
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services
            .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

        builder.Services.AddIdentityServer()
            .AddApiAuthorization<User, ApplicationDbContext>(options =>
            {
                options.IdentityResources["openid"].UserClaims.Add("role");
                options.ApiResources.Single().UserClaims.Add("role");
                options.IdentityResources["openid"].UserClaims.Add("permissions");
                options.ApiResources.Single().UserClaims.Add("permissions");
            });

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

        builder.Services.AddAuthentication()
            .AddIdentityServerJwt();

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Services.AddSignalR();

        builder.Services.AddOpenApiDocument(configure =>
        {
            configure.Title = "AHeatWeb";
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        //builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<DbInitializer>();

        builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, FlexibleAuthorizationPolicyProvider>();

        builder.Services.AddScoped<DiscoverShelly1Service>();
        builder.Services.AddScoped<DiscoverShelly2Service>();
        builder.Services.AddScoped<StrategyDiscoverService>(provider => (deviceType) =>
        {
            switch (deviceType)
            {
                case DeviceTypes.Generic:
                    throw new NotImplementedException();
                case DeviceTypes.ShellyGen1:
                    return provider.GetRequiredService<DiscoverShelly1Service>();
                case DeviceTypes.ShellyGen2:
                    return provider.GetRequiredService<DiscoverShelly2Service>();
                default:
                    throw new NotImplementedException();
            }
        });

        builder.Services.AddScoped<Shelly2DeviceService>();
        builder.Services.AddScoped<Shelly1DeviceService>();
        builder.Services.AddScoped<StrategyDeviceService>(provider => (deviceType) =>
        {
            switch (deviceType)
            {
                case DeviceTypes.Generic:
                    throw new NotImplementedException();
                case DeviceTypes.ShellyGen1:
                    return provider.GetRequiredService<Shelly1DeviceService>();
                case DeviceTypes.ShellyGen2:
                    return provider.GetRequiredService<Shelly2DeviceService>();
                default:
                    throw new NotImplementedException();
            }
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseWebAssemblyDebugging();
            //app.UseSwagger();
            //app.UseSwaggerUI();
            if (applyDbMigrationWithDataSeedFromProgramArguments)
            {
                using var scope = app.Services.CreateScope();

                var services = scope.ServiceProvider;

                var initializer = services.GetRequiredService<DbInitializer>();

                await initializer.RunAsync();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //app.UseHsts();
        }
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        //app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseIdentityServer();
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapRazorPages();
        app.MapControllers();
        app.MapHub<PowerStatusHub>("/statushub");
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}
