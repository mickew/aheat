using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AHeat.Web.API.Models.Config;

public class PowerDeviceConfiguration : IEntityTypeConfiguration<PowerDevice>
{
    public void Configure(EntityTypeBuilder<PowerDevice> builder)
    {
        builder.HasKey(p => p.ID);
        builder.Property(p => p.ID).ValueGeneratedOnAdd();
    }
}
