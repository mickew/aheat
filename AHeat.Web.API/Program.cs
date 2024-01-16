
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
using Serilog;
using Microsoft.Data.Sqlite;
using System;
using System.Configuration;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.Extensions.Options;

namespace AHeat.Web.API;

public class Program
{
    private const string SeedArgs = "--seed";

    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
      .AddEnvironmentVariables()
      .AddUserSecrets("0544df87-dce3-4caf-88f3-5636517b3dcd")
      .Build();

    public static async Task<int> Main(string[] args)
    {
        int res = 0;
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        try
        {
            Log.Information("Starting web host");
            var applyDbMigrationWithDataSeedFromProgramArguments = args.Any(x => x == SeedArgs);
            if (applyDbMigrationWithDataSeedFromProgramArguments)
            {
                try
                {
                    Log.Information("Migrating and Seeding data");
                    await SeedData();
                    Log.Information("Migrating and Seeding data done");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while migrating or initializing the database.");
                    return 1;
                }
                return 0;
            }
            WebApplicationBuilder builder = CreateBuilder(args);
            WebApplication app = CreateWebApp(builder);

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;
            var addressService = services.GetRequiredService<IApplicationHostAddressService>();
            var ipAddress = addressService.IpAddress;
            Log.Information("Application host address: {ipAddress}", ipAddress);

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            res = 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }

        return res;
    }

    private static WebApplication CreateWebApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseWebAssemblyDebugging();

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

        app.UseSerilogRequestLogging();
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
        return app;
    }

    private static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
        sqliteBuilder.DataSource = Path.Combine($"{builder.Environment.ContentRootPath}{Path.DirectorySeparatorChar}", sqliteBuilder.DataSource);
        var directory = Path.GetDirectoryName(sqliteBuilder.DataSource);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(sqliteBuilder.ToString()));
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
                options.IdentityResources["openid"].UserClaims.Add("firstname");
                options.ApiResources.Single().UserClaims.Add("firstname");
                options.IdentityResources["openid"].UserClaims.Add("lastname");
                options.ApiResources.Single().UserClaims.Add("lastname");
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

            //configure.AddSecurity("Bearer", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
            //{
            //    Type = NSwag.OpenApiSecuritySchemeType.OpenIdConnect,
            //    Flow = NSwag.OpenApiOAuth2Flow.Password,
            //    AuthorizationUrl = "connect/authorize",
                
            //    Scopes = new Dictionary<string, string>
            //    {
            //        { "AHeatWebAPI", "AHeatWeb API" }
            //    }
            //});
        });

        builder.Services.AddSingleton<IApplicationHostAddressService, ApplicationHostAddressService>();

        builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        builder.Services.AddSingleton<IAuthorizationPolicyProvider, FlexibleAuthorizationPolicyProvider>();

        builder.Services.AddScoped<DiscoverShelly1Service>();
        builder.Services.AddScoped<DiscoverShelly2Service>();
        builder.Services.AddScoped<StrategyDiscoverService>(provider => (deviceType) =>
        {
            return deviceType switch
            {
                DeviceTypes.Generic => throw new NotImplementedException(),
                DeviceTypes.ShellyGen1 => provider.GetRequiredService<DiscoverShelly1Service>(),
                DeviceTypes.ShellyGen2 => provider.GetRequiredService<DiscoverShelly2Service>(),
                _ => throw new NotImplementedException(),
            };
        });

        builder.Services.AddScoped<Shelly2DeviceService>();
        builder.Services.AddScoped<Shelly1DeviceService>();
        builder.Services.AddScoped<StrategyDeviceService>(provider => (deviceType) =>
        {
            return deviceType switch
            {
                DeviceTypes.Generic => throw new NotImplementedException(),
                DeviceTypes.ShellyGen1 => provider.GetRequiredService<Shelly1DeviceService>(),
                DeviceTypes.ShellyGen2 => provider.GetRequiredService<Shelly2DeviceService>(),
                _ => throw new NotImplementedException(),
            };
        });
        return builder;
    }

    private static async Task SeedData()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
        OperationalStoreOptions operationalStoreOptions = new();
        var options = Options.Create(operationalStoreOptions);
        
        await using var dbContext = new ApplicationDbContext(optionsBuilder.Options, options);
        var initializer = new DbInitializer(dbContext);
        await initializer.SeedAsync();
    }   
}
