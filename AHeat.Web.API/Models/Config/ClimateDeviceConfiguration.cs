using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AHeat.Web.API.Models.Config;

public class ClimateDeviceConfiguration : IEntityTypeConfiguration<ClimateDevice>
{
    public void Configure(EntityTypeBuilder<ClimateDevice> builder)
    {
        builder.HasKey(p => p.ID);
        builder.Property(p => p.ID).ValueGeneratedOnAdd();
        builder.Property(p => p.DeviceId).IsRequired();
        builder.HasIndex(p => p.DeviceId).IsUnique();
        builder.Property(p => p.Name).IsRequired();
    }
}
