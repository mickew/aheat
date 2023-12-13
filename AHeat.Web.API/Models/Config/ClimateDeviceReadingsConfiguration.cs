using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AHeat.Web.API.Models.Config;

public class ClimateDeviceReadingsConfiguration : IEntityTypeConfiguration<ClimateDeviceReading>
{
    public void Configure(EntityTypeBuilder<ClimateDeviceReading> builder)
    {
        builder.HasKey(p => p.ID);
        builder.Property(p => p.ID).ValueGeneratedOnAdd();
        builder.Property(p => p.Temperature).IsRequired(false);
        builder.Property(p => p.Humidity).IsRequired(false);
        builder.Property(p => p.Time).IsRequired();
        builder.HasOne(p => p.ClimateDevice).WithMany(p => p.Readings).HasForeignKey(p => p.ClimateDeviceID).OnDelete(DeleteBehavior.Cascade);
    }
}
