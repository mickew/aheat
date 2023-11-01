namespace AHeat.Application.Models.Shelly;
internal record ChannelStatus(
     int Id,
     string Source,
     bool Output,
     DeviceTemperature Temperature
 );

public record DeviceTemperature(
    double TC,
    double TF
);
