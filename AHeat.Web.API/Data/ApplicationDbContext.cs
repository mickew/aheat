using AHeat.Web.API.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace AHeat.Web.API.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<User, Role>
{
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }

    public DbSet<PowerDevice> PowerDevices { get; set; }
    public DbSet<ClimateDevice> ClimateDevices { get; set; }
    public DbSet<ClimateDeviceReading> ClimateDeviceReadings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
